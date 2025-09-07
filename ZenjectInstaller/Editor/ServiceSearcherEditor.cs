using UnityEditor;
using UnityEngine;
using ZenjectInstaller.TypeSearchers;

namespace ZenjectInstaller.Editor
{
    [CustomEditor(typeof(ServiceSearcher))]
    public class ServiceSearcherEditor : UnityEditor.Editor
    {
        private ServiceSearcher _searcher;

        private void OnEnable()
        {
            _searcher = (ServiceSearcher)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
        
            if (GUILayout.Button("SearchServices"))
            {
                _searcher.Search();
            }
            if (GUILayout.Button("ResetServices"))
            {
                _searcher.Reset();
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}