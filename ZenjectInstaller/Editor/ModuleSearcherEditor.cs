using UnityEditor;
using UnityEngine;
using ZenjectInstaller.TypeSearchers;

namespace ZenjectInstaller.Editor
{
    [CustomEditor(typeof(ModuleSearcher))]
    public class ModuleSearcherEditor : UnityEditor.Editor
    {
        private ModuleSearcher _searcher;

        private void OnEnable()
        {
            _searcher = (ModuleSearcher)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
        
            if (GUILayout.Button("SearchModules"))
            {
                _searcher.Search();
                EditorUtility.SetDirty(_searcher);
            }
            if (GUILayout.Button("ResetModules"))
            {
                _searcher.Reset();
                EditorUtility.SetDirty(_searcher);
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}