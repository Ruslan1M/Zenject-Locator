namespace ZenjectInstaller.Contracts
{
    public interface IBinder {
        IBindFluent Bind<TContract, TImpl>() where TImpl : TContract;
        IBindFluent BindComponentOnNewGo<TContract, TComp>(string goName = null, bool dontDestroy = true)
            where TComp : UnityEngine.MonoBehaviour, TContract;
        IBindFactoryFluent BindFactory<TValue, TFactory>();  
        void Raw(string commentOrPseudoCode);
    }
}