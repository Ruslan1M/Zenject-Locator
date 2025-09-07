using ZenjectInstaller.Enums;

namespace ZenjectInstaller.Bind
{
    public sealed class BindSpec {
        public string Kind;                        
        public string ContractAQN, ImplAQN;
        public string Id, WhenInjectedIntoAQN;
        public DiScope Scope = DiScope.Single;
        public DiContext Context = DiContext.Project;
        public System.Collections.Generic.Dictionary<string,string> Meta = new();
    }
}