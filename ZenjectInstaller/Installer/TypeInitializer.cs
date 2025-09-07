using System;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace ZenjectInstaller.Installer
{
    public static class TypeInitializer
    {
        public static void InitializeService(Type serviceType, DiContainer container, Transform parentTransform)
        {
            MethodInfo initMethod = serviceType.GetMethod("InitializeService", BindingFlags.Static | BindingFlags.Public);
            
            if (initMethod != null) 
                initMethod.Invoke(null, new object[] { container, parentTransform});
            else
                Debug.LogError($"Cannot initialize service with type {serviceType}, cant find 'InitializeService' method");
        }
    
        public static void InitializeModule(Type moduleType, DiContainer container)
        {
            MethodInfo initMethod = moduleType.GetMethod("InitializeModule", BindingFlags.Static | BindingFlags.Public);
            
            if (initMethod != null) 
                initMethod.Invoke(null, new object[] {container });
            else
                Debug.LogError($"Cannot initialize module with type {moduleType}, cant find 'InitializeModule' method");
        }
    }
}