using System;

namespace ZenjectInstaller.TypeSearchers
{
    [Serializable]
    public class ServiceInfo : SearchableItemInfo
    {
        public ServiceInfo(Type type) : base(type)
        {
        
        }
    }
}