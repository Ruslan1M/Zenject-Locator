using System;
using UnityEngine;

namespace ZenjectInstaller.TypeSearchers
{
    [CreateAssetMenu(fileName = "ModuleSearcher", menuName = "ViraGames/Infrastructure/ModuleSearcher", order = 0)]
    public class ModuleSearcher : TypeSearcher<IModule, ModuleInfo>
    {
        protected override ModuleInfo CreateNewInfo(Type type) => 
            new ModuleInfo(type);
    }
}