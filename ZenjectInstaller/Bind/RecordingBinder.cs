using System;
using ZenjectInstaller.Contracts;
using ZenjectInstaller.Enums;

namespace ZenjectInstaller.Bind
{
    public sealed class RecordingBinder : IBinder, IBindFluent, IBindFactoryFluent
    {
        public readonly System.Collections.Generic.List<BindSpec> Specs = new();
        BindSpec _cur;
        
        readonly string _moduleName;

        public RecordingBinder(string moduleName = null) { _moduleName = moduleName; }
        
        BindSpec NewSpec(string kind, Type contract, Type impl)
        {
            var s = new BindSpec {
                Kind = kind,
                ContractAQN = contract?.AssemblyQualifiedName,
                ImplAQN = impl?.AssemblyQualifiedName,
                Context = DiContext.Project
            };
            s.Meta["Module"] = _moduleName ?? "";
            Specs.Add(s);
            return s;
        }
        
        public IBindFluent Bind<TContract, TImpl>() where TImpl : TContract
        {
            _cur = NewSpec("Provides", typeof(TContract), typeof(TImpl));
            return this;
        }

        public IBindFluent BindComponentOnNewGo<TContract, TComp>(string goName = null, bool dontDestroy = true)
            where TComp : UnityEngine.MonoBehaviour, TContract
        {
            _cur = NewSpec("ComponentOnNewGo", typeof(TContract), typeof(TComp));
            _cur.Meta["GoName"] = goName ?? typeof(TComp).Name;
            _cur.Meta["DontDestroy"] = dontDestroy.ToString();
            return this;
        }

        public IBindFactoryFluent BindFactory<TValue, TFactory>()
        {
            _cur = NewSpec("Factory", typeof(TValue), typeof(TFactory));
            return this;
        }

        public IBindFluent Id(string id)
        {
            _cur.Id = id;
            return this;
        }

        public IBindFluent Scope(DiScope s)
        {
            _cur.Scope = s;
            return this;
        }

        public IBindFluent WhenInjectedInto<T>()
        {
            _cur.WhenInjectedIntoAQN = typeof(T).AssemblyQualifiedName;
            return this;
        }

        public IBindFluent NonLazy()
        {
            _cur.Meta["NonLazy"] = "true";
            return this;
        }

        public IBindFluent FromComponentInNewPrefabResource(string path)
        {
            _cur.Meta["PrefabRes"] = path;
            return this;
        }

        public IBindFluent FromComponentInNewPrefabGUID(string field)
        {
            _cur.Meta["PrefabGuidField"] = field;
            return this;
        }

        public IBindFactoryFluent FromNew()
        {
            _cur.Meta["From"] = "New";
            return this;
        }

        public IBindFactoryFluent FromMethod(string providerAQN, string method)
        {
            _cur.Meta["From"] = "Method";
            _cur.Meta["Provider"] = providerAQN;
            _cur.Meta["Method"] = method;
            return this;
        }

        IBindFactoryFluent IBindFactoryFluent.FromComponentInNewPrefabResource(string path)
        {
            _cur.Meta["From"] = "PrefabRes";
            _cur.Meta["Path"] = path;
            return this;
        }

        IBindFactoryFluent IBindFactoryFluent.FromComponentInNewPrefabGUID(string field)
        {
            _cur.Meta["From"] = "PrefabGuid";
            _cur.Meta["Field"] = field;
            return this;
        }

        public IBindFactoryFluent FromMemoryPool(params string[] meta)
        {
            _cur.Meta["From"] = "MemoryPool";
            return this;
        }

        public IBindFactoryFluent SubContainerWithInstaller(string installerAQN, string field)
        {
            _cur.Meta["From"] = "SubContainer";
            _cur.Meta["Installer"] = installerAQN;
            _cur.Meta["Field"] = field;
            return this;
        }

        public void Raw(string code)
        {
            Specs.Add(new BindSpec { Kind = "Raw", Meta = { { "Code", code } } });
        }
    }
}