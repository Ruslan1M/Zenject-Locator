using ZenjectInstaller.Enums;

namespace ZenjectInstaller.Contracts
{
    public interface IBindFluent {
        IBindFluent Id(string id);
        IBindFluent Scope(DiScope scope);
        IBindFluent WhenInjectedInto<T>();
        IBindFluent NonLazy();
        IBindFluent FromComponentInNewPrefabResource(string path);
        IBindFluent FromComponentInNewPrefabGUID(string guidFieldName);
    }
}