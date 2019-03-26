using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;
using Superpower.Model;
using ValveFormat.Superpower;

namespace Tf2Rebalance.CreateSummary
{
    public class Converter
    {
        private static ILogger Log = Serilog.Log.ForContext<Converter>();

        private readonly IDictionary<string, string> _weaponNames;
        private readonly IDictionary<string, string> _classNames = new Dictionary<string, string>
        {
            { "scout", "Scout" },
            { "soldier", "Soldier" },
            { "pyro", "Pyro" },
            { "demoman", "Demoman" },
            { "heavy", "Heavy" },
            { "engineer", "Engineer" },
            { "medic", "Medic" },
            { "sniper", "Sniper" },
            { "spy", "Spy" },
        };

        public Converter(IDictionary<string, string> weaponNames)
        {
            _weaponNames = weaponNames;
        }

        public string Execute(string input)
        {
            IList<Node> definitionNodes;
            string error;
            Position position;
            if (!ValveParser.TryParse(input, out definitionNodes, out error, out position))
            {
                Log.Error("Error parsing input: {Error} at {Position}", error, position);
                return null;
            }
            if (definitionNodes.Count == 0)
            {
                Log.Error("no data found in input");
                return null;
            }

            var stringBuilder = FindItemNodes(definitionNodes)
                .Select(i => new
                {
                    heading = CreateHeading(i.Parent.Name),
                    info = i.Value.Replace("\n", "\r\n"),
                })
                .Aggregate(new StringBuilder(), (sb, x) =>
                {
                    sb.AppendLine(x.heading);
                    sb.AppendLine(x.info);
                    sb.AppendLine();

                    return sb;
                });

            return stringBuilder.ToString();
        }

        private IEnumerable<Node> FindItemNodes(IEnumerable<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                if (node.Name.ToLowerInvariant() == "info")
                {
                    yield return node;
                }

                if (node.Childs == null)
                    continue;

                foreach (Node childNode in FindItemNodes(node.Childs))
                {
                    yield return childNode;
                }
            }
        }

        private string CreateHeading(string text)
        {
            var weaponNames = text.Split(';')
                .Select(s => s.Trim())
                .Select(id => _weaponNames.ContainsKey(id.ToLowerInvariant()) ? _weaponNames[id]: id)
                .Select(id => _classNames.ContainsKey(id.ToLowerInvariant()) ? _classNames[id]: id)
                .ToList();

            string heading = string.Join(", ", weaponNames);
            return heading;
        }
    }
}