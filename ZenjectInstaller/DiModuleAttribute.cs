namespace ZenjectInstaller
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class DiModuleAttribute : System.Attribute
    {
        public string Name;

        public DiModuleAttribute(string name = null)
        {
            Name = name;
        }
    }
}