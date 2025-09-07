using System;
using UnityEngine;

namespace ZenjectInstaller.TypeSearchers
{
    [Serializable]
    public class SearchableItemInfo
    {
        public readonly Type ItemType;
        public string ItemName;
        
        [HideInInspector]
        public string ItemFullName;

        public SearchableItemInfo(Type type)
        {
            ItemType = type;
            ItemName = type.Name;
            ItemFullName = type.FullName;
        }

        public override string ToString() => ItemName;

        public override bool Equals(object obj)
        {
            if (obj is SearchableItemInfo other)
            {
                return ItemType == other.ItemType;
            }
            return false;
        }

        public override int GetHashCode() => 
            ItemType?.GetHashCode() ?? 0;
    }
}