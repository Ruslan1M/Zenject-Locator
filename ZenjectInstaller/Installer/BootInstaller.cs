using System;
using UnityEngine;
using Zenject;
using ZenjectInstaller.TypeSearchers;

namespace ZenjectInstaller.Installer
{
    public class BootInstaller : MonoInstaller
    {
        public ServiceSearcher serviceSearcher;
        public ModuleSearcher moduleSearcher;
        
        public override void InstallBindings()
        {
            InitializeServices();
            InitializeModules();
        }

        private void InitializeServices()
        {
            TypeInitialize<IService>(serviceSearcher.itemInfos.ToArray(), (type) =>
            {
                TypeInitializer.InitializeService(type, Container, transform);
            });
        }

        private void InitializeModules()
        {
            TypeInitialize<IModule>(moduleSearcher.itemInfos.ToArray(), (type) =>
            {
                TypeInitializer.InitializeModule(type, Container);
            });
        }

        private void TypeInitialize<T>(SearchableItemInfo[] typeNames, Action<Type> Initialize)
        {
            foreach (var name in typeNames)
            {
                Type serviceType = Type.GetType(name.ItemFullName);

                if (serviceType != null && typeof(T).IsAssignableFrom(serviceType))
                {
                    Initialize.Invoke(serviceType);
                }
                else
                {
                    Debug.LogError($"Cannot find type: {name}");
                }
            }
        }
    }
}