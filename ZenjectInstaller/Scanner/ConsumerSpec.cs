using ZenjectInstaller.Enums;

namespace ZenjectInstaller.Scanner
{
    public sealed class ConsumerSpec
    {
        public string ConsumerAQN;
        public DiContext Context;
        public string ModuleName;
        public System.Collections.Generic.List<ConsumeReq> Requires = new();
    }
}