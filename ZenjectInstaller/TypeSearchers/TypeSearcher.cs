using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ZenjectInstaller.TypeSearchers
{
    public abstract class TypeSearcher<Ttype, Tinfo> : ScriptableObject, ITypeSearcher
        where Tinfo : SearchableItemInfo
    {
        public List<Tinfo> itemInfos = new List<Tinfo>();

        public void Search()
        {
            itemInfos.Clear();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (!typeof(Ttype).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract) 
                        continue;

                    itemInfos.Add(CreateNewInfo(type));
                }
            }
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void Reset()
        {
            itemInfos.Clear();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        protected abstract Tinfo CreateNewInfo(Type type);
    }
}