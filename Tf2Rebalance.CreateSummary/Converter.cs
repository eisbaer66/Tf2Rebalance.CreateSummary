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

        public Converter(IDictionary<string, string> weaponNames)
        {
            _weaponNames = weaponNames;
        }

        public string Execute(string input)
        {
            StringBuilder stringBuilder = new StringBuilder();

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
            if (definitionNodes.Count > 1)
            {
                Log.Error("multiple root-nodes found in input");
                return null;
            }

            Node root = definitionNodes[0];
            if (root.Name != "tf2rebalance_attributes")
            {
                Log.Error("found root-node {ActualRootName} but excpected {ExpectedRootName}", root.Name, "tf2rebalance_attributes");
                return null;
            }

            foreach (Node definitionNode in root.Childs)
            {
                string heading = CreateHeading(definitionNode);
                string info = CreateInfo(definitionNode);

                stringBuilder.AppendLine(heading);
                stringBuilder.AppendLine(info);
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        private static string CreateInfo(Node definitionNode)
        {
            Node infoNode = definitionNode.Childs.FirstOrDefault(n => n.Name == "info");
            string info = infoNode.Value;
            info = info.Replace("\n", "\r\n");
            return info;
        }

        private string CreateHeading(Node definitionNode)
        {
            string key = definitionNode.Name;
            var weaponNames = key.Split(';')
                .Select(s => s.Trim())
                .Where(id => _weaponNames.ContainsKey(id))
                .Select(id => _weaponNames[id])
                .ToList();

            string heading = string.Join(", ", weaponNames);
            return heading;
        }
    }
}