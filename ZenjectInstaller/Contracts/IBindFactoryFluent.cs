namespace ZenjectInstaller.Contracts
{
    public interface IBindFactoryFluent {
        IBindFactoryFluent FromNew();
        IBindFactoryFluent FromMethod(string providerTypeAQN, string methodName);
        IBindFactoryFluent FromComponentInNewPrefabResource(string path);
        IBindFactoryFluent FromComponentInNewPrefabGUID(string fieldName);
        IBindFactoryFluent FromMemoryPool(params string[] meta);             
        IBindFactoryFluent SubContainerWithInstaller(string installerTypeAQN, string prefabGuidFieldName);
    }
}