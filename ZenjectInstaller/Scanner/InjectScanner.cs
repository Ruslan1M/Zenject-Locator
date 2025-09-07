#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZenjectInstaller.Bind;
using ZenjectInstaller.Enums;

namespace ZenjectInstaller.Scanner
{
    public static class InjectScanner
    {
        static readonly Dictionary<string, Type> _aqnToType = new();
        static readonly Dictionary<Type, List<ReqKey>> _reqCache = new();
        static readonly Dictionary<Type, (ConstructorInfo ctor, Zenject.InjectAttribute attr)> _ctorCache = new();

        const BindingFlags Inst = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        struct ReqKey
        {
            public Type Contract;
            public string Id;
            public bool Optional;
        }

        public static List<ConsumerSpec> ScanConsumers(IEnumerable<BindSpec> binds, string moduleNameFallback = null)
        {
            if (binds == null) return new List<ConsumerSpec>(0);

            var keys = new HashSet<(Type type, DiContext ctx, string module)>();

            foreach (var b in binds)
            {
                if (!string.IsNullOrEmpty(b.ImplAQN))
                {
                    var t = ResolveType(b.ImplAQN);
                    if (t != null)
                        keys.Add((t, b.Context, GetModule(b, moduleNameFallback)));
                }

                if (b.Kind == "Factory" && !string.IsNullOrEmpty(b.ContractAQN))
                {
                    var vt = ResolveType(b.ContractAQN);
                    if (vt != null)
                        keys.Add((vt, b.Context, GetModule(b, moduleNameFallback)));
                }
            }

            var result = new List<ConsumerSpec>(keys.Count);
            foreach (var key in keys)
            {
                var reqs = GetRequirementsForType(key.type);
                if (reqs.Count == 0) continue;

                var spec = new ConsumerSpec
                {
                    ConsumerAQN = key.type.AssemblyQualifiedName,
                    Context = key.ctx,
                    ModuleName = key.module,
                    Requires = new List<ConsumeReq>(reqs.Count)
                };

                foreach (var r in reqs)
                {
                    spec.Requires.Add(new ConsumeReq
                    {
                        ContractAQN = r.Contract.AssemblyQualifiedName,
                        Id = r.Id,
                        Optional = r.Optional
                    });
                }

                result.Add(spec);
            }

            return result;
        }

        private static List<ReqKey> GetRequirementsForType(Type t)
        {
            if (t == null) return new List<ReqKey>(0);
            if (_reqCache.TryGetValue(t, out var cached)) return cached;

            var list = new List<ReqKey>(8);

            foreach (var f in t.GetFields(Inst))
            {
                if (HasInjectOrOptional(f, out var id, out var opt))
                    list.Add(new ReqKey { Contract = Canonical(f.FieldType), Id = id, Optional = opt });
            }

            foreach (var p in t.GetProperties(Inst))
            {
                if (p.GetIndexParameters().Length != 0) continue;
                if (HasInjectOrOptional(p, out var id, out var opt))
                    list.Add(new ReqKey { Contract = Canonical(p.PropertyType), Id = id, Optional = opt });
            }

            foreach (var m in t.GetMethods(Inst))
            {
                var inj = m.GetCustomAttribute<Zenject.InjectAttribute>();
                if (inj == null) continue;

                foreach (var pr in m.GetParameters())
                {
                    var pinj = pr.GetCustomAttribute<Zenject.InjectAttribute>();
                    var opt = pr.GetCustomAttribute<Zenject.InjectOptionalAttribute>() != null;
                    list.Add(new ReqKey
                    {
                        Contract = Canonical(pr.ParameterType),
                        Id = ToId(pinj?.Id ?? inj.Id),
                        Optional = opt
                    });
                }
            }

            var (ctor, ctorAttr) = ChooseCtorCached(t);
            if (ctor != null)
            {
                foreach (var pr in ctor.GetParameters())
                {
                    var pinj = pr.GetCustomAttribute<Zenject.InjectAttribute>();
                    var opt = pr.GetCustomAttribute<Zenject.InjectOptionalAttribute>() != null;
                    list.Add(new ReqKey
                    {
                        Contract = Canonical(pr.ParameterType),
                        Id = ToId(pinj?.Id ?? ctorAttr?.Id),
                        Optional = opt
                    });
                }
            }

            if (list.Count > 1)
            {
                var set = new HashSet<(Type, string, bool)>();
                var dedup = new List<ReqKey>(list.Count);
                foreach (var r in list)
                    if (set.Add((r.Contract, r.Id, r.Optional)))
                        dedup.Add(r);
                list = dedup;
            }

            _reqCache[t] = list;
            return list;
        }


        private static string GetModule(BindSpec b, string fallback) =>
            (b.Meta != null && b.Meta.TryGetValue("Module", out var m)) ? m : fallback;

        private static string ToId(object id) => id == null ? null : id.ToString();

        private static bool HasInjectOrOptional(MemberInfo mi, out string idStr, out bool optional)
        {
            var inj = mi.GetCustomAttribute<Zenject.InjectAttribute>();
            var opt = mi.GetCustomAttribute<Zenject.InjectOptionalAttribute>();
            idStr = ToId(inj?.Id);
            optional = opt != null;
            return inj != null || opt != null;
        }

        private static Type Canonical(Type raw)
        {
            if (raw == null) return null;
            if (raw.IsGenericType)
            {
                var def = raw.GetGenericTypeDefinition();
                if (def == typeof(List<>) || def == typeof(IEnumerable<>) ||
                    (def.FullName?.StartsWith("Zenject.Lazy", StringComparison.Ordinal) ?? false))
                    return raw.GetGenericArguments()[0];
            }

            return raw;
        }

        private static (ConstructorInfo ctor, Zenject.InjectAttribute attr) ChooseCtorCached(Type t)
        {
            if (_ctorCache.TryGetValue(t, out var hit)) return hit;

            var ctors = t.GetConstructors(Inst);
            if (ctors.Length == 0) return _ctorCache[t] = (null, null);

            var withAttr = (from c in ctors
                let a = c.GetCustomAttribute<Zenject.InjectAttribute>()
                where a != null
                select (ctor: c, attr: a)).ToArray();

            if (withAttr.Length == 1) return _ctorCache[t] = withAttr[0];
            if (withAttr.Length > 1)
                return _ctorCache[t] = withAttr.OrderByDescending(e => e.ctor.GetParameters().Length).First();

            return _ctorCache[t] = (ctors.OrderByDescending(c => c.GetParameters().Length).First(), null);
        }

        private static Type ResolveType(string aqn)
        {
            if (string.IsNullOrEmpty(aqn)) return null;
            if (_aqnToType.TryGetValue(aqn, out var t)) return t;

            t = Type.GetType(aqn);

            if (t == null)
            {
                var shortName = aqn.Split(',')[0];
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.IsDynamic) continue;
                    t = asm.GetType(shortName);
                    if (t != null) break;
                }
            }

            _aqnToType[aqn] = t;
            return t;
        }
    }
}
#endif