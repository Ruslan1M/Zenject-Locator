using System;
using UnityEngine;

namespace ZenjectInstaller.TypeSearchers
{
    [CreateAssetMenu(fileName = "ServiceSearcher", menuName = "ViraGames/Infrastructure/ServiceSearcher", order = 0)]
    public class ServiceSearcher : TypeSearcher<IService, ServiceInfo>
    {
        protected override ServiceInfo CreateNewInfo(Type type) => 
            new ServiceInfo(type);
    }
}