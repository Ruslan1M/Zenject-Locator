#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using ZenjectInstaller.Bind;
using ZenjectInstaller.Contracts;

namespace ZenjectInstaller.DiGraph
{
    public static class DiSimulator
    {
        public static List<BindSpec> SimulateAll(Func<IEnumerable<Type>> moduleTypesProvider)
        {
            var allSpecs = new List<BindSpec>();

            foreach (var t in moduleTypesProvider())
            {
                var modAttr = t.GetCustomAttribute<DiModuleAttribute>();
                var moduleName = modAttr?.Name ?? t.Name;

                var mi = t.GetMethod("Describe",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                if (mi == null)
                {
                    UnityEngine.Debug.LogWarning($"[DI Sim] {t.FullName}: not found static Describe(IBinder)");
                    continue;
                }

                var ps = mi.GetParameters();
                if (ps.Length != 1 || ps[0].ParameterType != typeof(IBinder) || mi.IsGenericMethodDefinition)
                {
                    UnityEngine.Debug.LogWarning($"[DI Sim] {t.FullName}.Describe has an inappropriate signature");
                    continue;
                }
            
                var rec = new RecordingBinder(moduleName);

                try
                {
                    mi.Invoke(null, new object[] { rec });
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"[DI Sim] {t.FullName}.Describe() exception:\n{ex}");
                    continue;
                }

                allSpecs.AddRange(rec.Specs);
            }

            return allSpecs;
        }
    }
}
#endif