using System;

namespace ZenjectInstaller.TypeSearchers
{
    [Serializable]
    public class ModuleInfo : SearchableItemInfo
    {
        public ModuleInfo(Type type) : base(type)
        {
            
        }
    }
}