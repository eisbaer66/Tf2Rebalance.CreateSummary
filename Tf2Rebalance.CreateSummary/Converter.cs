using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Serilog;
using Superpower.Model;
using ValveFormat.Superpower;

namespace Tf2Rebalance.CreateSummary
{
    public class RebalanceInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string info { get; set; }
        public string category { get; set; }
        public string itemclass { get; set; }
        public string slot { get; set; }
    }

    public class Converter
    {
        private static ILogger Log = Serilog.Log.ForContext<Converter>();

        private readonly IDictionary<string, List<ItemInfo>> _itemInfos;
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

        public Converter(IDictionary<string, List<ItemInfo>> itemInfos)
        {
            _itemInfos = itemInfos;
        }

        public IEnumerable<RebalanceInfo> Execute(string input)
        {
            IList<Node> definitionNodes;
            string error;
            Position position;
            bool successfullParse = ValveParser.TryParse(input, out definitionNodes, out error, out position);
            if (!successfullParse)
            {
                Log.Error("Error parsing input: {Error} at {Position}", error, position);
                return null;
            }
            if (definitionNodes.Count == 0)
            {
                Log.Error("no data found in input");
                return null;
            }

            var groupings = FindItemNodes(definitionNodes)
                .SelectMany(node => FindAllInfos(node.Parent.Name)
                    .Select(i => new RebalanceInfo
                    {
                        id = i.Id,
                        name = _classNames.ContainsKey(i.Name.ToLowerInvariant()) ? _classNames[i.Name.ToLowerInvariant()] : i.Name,
                        info = node.Value.Replace("\n", "\r\n"),
                        category = i.Category,
                        itemclass = NormalizeClassName(i.Class),
                        slot = i.Slot
                    }));

            return groupings;
        }

        private string NormalizeClassName(string className)
        {
            if (className == null)
                return null;

            if (!_classNames.ContainsKey(className.ToLowerInvariant()))
                return className;

            return _classNames[className.ToLowerInvariant()];
        }

        private IEnumerable<ItemInfo> FindAllInfos(string text)
        {
            return text.Split(';')
                .Select(s => s.Trim())
                .SelectMany(id =>
                {
                    if (_classNames.ContainsKey(id.ToLowerInvariant()))
                    {
                        return new List<ItemInfo>
                        {
                            new ItemInfo
                            {
                                Name = _classNames[id.ToLowerInvariant()],
                                Id = id,
                                Category = "Classes"
                            }
                        };
                    }
                    if (!_itemInfos.ContainsKey(id))
                        return new List<ItemInfo>
                        {
                            new ItemInfo
                            {
                                Name = id,
                                Id = id,
                                Category = "unknown",
                            }
                        };

                    return _itemInfos[id];
                });
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
    }
}