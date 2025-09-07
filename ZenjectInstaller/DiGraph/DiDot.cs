#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZenjectInstaller.Bind;
using ZenjectInstaller.Enums;

namespace ZenjectInstaller.DiGraph
{
    public static class DiDot
    {
        public static string Export(List<BindSpec> specs)
        {
            var sb = new StringBuilder();
            sb.AppendLine("digraph DI { rankdir=LR; fontsize=10;");
            foreach (var s in specs.Where(s => s.Kind != "Raw"))
            {
                var prov = NodeId("prov", s.ImplAQN, s.Id, s.Context);
                var cont = NodeId("contract", s.ContractAQN, null, s.Context);
                sb.AppendLine(
                    $"{prov} [label=\"{TypeName(s.ImplAQN)}\\n{s.Scope} {(s.Id != null ? $"\\nId:{s.Id}" : "")}\", shape=box];");
                sb.AppendLine($"{cont} [label=\"{TypeName(s.ContractAQN)}\", shape=ellipse];");
                sb.AppendLine($"{prov} -> {cont};");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        static string NodeId(string kind, string aqn, string id, DiContext ctx) => $"\"{kind}:{TypeName(aqn)}:{id}:{ctx}\"";
        static string TypeName(string aqn) => aqn?.Split(',')[0];
    }
}
#endif