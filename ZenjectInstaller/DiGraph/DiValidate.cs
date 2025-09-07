#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using ZenjectInstaller.Bind;
using ZenjectInstaller.Scanner;

namespace ZenjectInstaller.DiGraph
{
    public struct DiIssue
    {
        public string Severity;
        public string Message;
    }

    public static class DiValidate
    {
        public static List<DiIssue> Validate(IEnumerable<BindSpec> specs)
        {
            var issues = new List<DiIssue>();

            var groups = specs.Where(s => s.Kind != "Raw").GroupBy(s => (s.ContractAQN, s.Id ?? "", s.Context));
            foreach (var g in groups)
                if (g.Count() > 1)
                    issues.Add(new DiIssue
                    {
                        Severity = "Warning",
                        Message =
                            $"Duplicate providers for {Short(g.Key.ContractAQN)} id='{g.Key.Item2}' ctx={g.Key.Context}"
                    });

            foreach (var s in specs.Where(s => s.Kind == "Provides" || s.Kind.StartsWith("Component")))
            {
                var c = System.Type.GetType(s.ContractAQN);
                var i = System.Type.GetType(s.ImplAQN);
                if (c == null || i == null || !c.IsAssignableFrom(i))
                    issues.Add(new DiIssue
                        { Severity = "Error", Message = $"Incompatible: {Short(s.ContractAQN)} ← {Short(s.ImplAQN)}" });
            }

            return issues;
        }

        public static List<DiIssue> CheckConsumers(IEnumerable<BindSpec> specs, IEnumerable<ConsumerSpec> consumer)
        {
            var issues = new List<DiIssue>();

            var providers = specs
                .Where(p => !string.IsNullOrEmpty(p.ContractAQN))
                .GroupBy(p => (p.ContractAQN, p.Id ?? "", p.Context))
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var c in consumer)
            {
                foreach (var r in c.Requires)
                {
                    var key = (r.ContractAQN, r.Id ?? "", c.Context);
                    if (!providers.TryGetValue(key, out var provs) || provs.Count == 0)
                    {
                        issues.Add(new DiIssue()
                        {
                            Severity = "Warning",
                            Message =
                                $"❌ Missing provider for {Short(r.ContractAQN)} {(string.IsNullOrEmpty(r.Id) ? "" : $"(id:{r.Id})")}  (consumer: {Short(c.ConsumerAQN)}, ctx: {c.Context})"
                        });
                    }
                    else if (provs.Count > 1)
                    {
                        issues.Add(new DiIssue()
                        {
                            Severity = "Warning",
                            Message =
                                $"⚠ Ambiguous providers for {Short(r.ContractAQN)} {(string.IsNullOrEmpty(r.Id) ? "" : $"(id:{r.Id})")}  (consumer: {Short(c.ConsumerAQN)}, found: {provs.Count})"
                        });
                    }
                }
            }

            return issues;
        }


        static string Short(string aqn) => aqn?.Split(',')[0];
    }
#endif
}