#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ZenjectInstaller.Bind;
using ZenjectInstaller.Scanner;

namespace ZenjectInstaller.DiGraph
{
    public class DiGraphWindow : EditorWindow
    {
        private List<ConsumerSpec> _consumers = new();
        private List<BindSpec> _specs = new();
        private Vector2 _scroll;

        [MenuItem("Tools/DI/Graph")]
        private static void Open() => GetWindow<DiGraphWindow>("DI Graph");

        private void OnEnable() => Resimulate();

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Simulate")) Resimulate();
            if (GUILayout.Button("Validate")) ShowIssues();
            if (GUILayout.Button("Export DOT")) ExportDot();
            EditorGUILayout.EndHorizontal();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            if (_specs.Count == 0)
                EditorGUILayout.LabelField("No specs. Click Simulate.", EditorStyles.centeredGreyMiniLabel);

            foreach (var group in _specs.GroupBy(s => s.Meta.TryGetValue("Module", out var m) ? m : ""))
            {
                EditorGUILayout.LabelField(string.IsNullOrEmpty(group.Key) ? "Module (unknown)" : group.Key, EditorStyles.boldLabel);

                EditorGUILayout.LabelField("Providers:", EditorStyles.miniBoldLabel);
                foreach (var s in group)
                    EditorGUILayout.LabelField($"  {s.Kind} [{s.Context}]  {Short(s.ImplAQN)} → {Short(s.ContractAQN)} {(string.IsNullOrEmpty(s.Id) ? "" : $"(id:{s.Id})")}");

                var cons = _consumers.Where(c => c.ModuleName == group.Key);
                if (cons.Any())
                {
                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("Consumers:", EditorStyles.miniBoldLabel);
                    foreach (var c in cons)
                    {
                        EditorGUILayout.LabelField($"  {Short(c.ConsumerAQN)} requires:");
                        foreach (var r in c.Requires)
                            EditorGUILayout.LabelField($"    • {Short(r.ContractAQN)} {(string.IsNullOrEmpty(r.Id) ? "" : $"(id:{r.Id})")} {(r.Optional ? "[optional]" : "")}");
                    }
                }

                EditorGUILayout.Space(8);
            }

            EditorGUILayout.EndScrollView();
        }
    
        private void Resimulate()
        {
            var types = FindModuleTypes().ToList();
            Debug.Log($"[DI] Modules: {types.Count}");
            _specs = DiSimulator.SimulateAll(() => types).ToList();

            _consumers = InjectScanner.ScanConsumers(_specs).ToList();

            Debug.Log($"[DI] Providers: {_specs.Count}, Consumers: {_consumers.Count}");
            Repaint();
        }


        private static IEnumerable<Type> FindModuleTypes()
        {
            return TypeCache.GetTypesWithAttribute<DiModuleAttribute>();
        }

        private void ShowIssues()
        {
            var issues = DiValidate.Validate(_specs);
            var consumersIssues = DiValidate.CheckConsumers(_specs, _consumers);
        
            issues.AddRange(consumersIssues);
        
            EditorUtility.DisplayDialog("Validate", issues.Count == 0 ? "No issues" : string.Join("\n", issues.Select(i => $"[{i.Severity}] {i.Message}")), "OK");
        }

        private void ExportDot()
        {
            var dot = DiDot.Export(_specs);
            var path = EditorUtility.SaveFilePanel("Save .dot", "", "digraph.dot", "dot");
            if (!string.IsNullOrEmpty(path)) System.IO.File.WriteAllText(path, dot);
        }

        static string Short(string aqn) => string.IsNullOrEmpty(aqn) ? "?" : aqn.Split(',')[0];
    }
}
#endif
