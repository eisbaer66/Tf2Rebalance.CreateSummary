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

        public string Execute(string input)
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
                    }))
                .GroupBy(x => new {x.category,     x.itemclass,     x.slot})
                .GroupBy(x => new {x.Key.category, x.Key.itemclass})
                .GroupBy(x => x.Key.category);

            StringBuilder builder = new StringBuilder();
            foreach (var classes in groupings)
            {
                builder.AppendLine(classes.Key);
                foreach (var slots in classes)
                {
                    string itemclass = slots.Key.itemclass;
                    if (!string.IsNullOrEmpty(itemclass))
                        builder.AppendLine(itemclass);

                    foreach (var weapons in slots.OrderBy(s => GetSlot(s.Key.slot)))
                    {
                        string slot = weapons.Key.slot;
                        if (!string.IsNullOrEmpty(slot))
                            builder.AppendLine(slot);

                        var groupedWeapons = weapons.GroupBy(x => x.info)
                            .Select(g =>
                            {
                                string name = string.Join(", ", g.Select(i => i.name));
                                var itemInfo = g.First();
                                itemInfo.name = name;
                                return itemInfo;
                            });
                        foreach (var weapon in groupedWeapons)
                        {
                            builder.AppendLine(weapon.name);
                            builder.AppendLine(weapon.info);
                            builder.AppendLine();
                        }
                    }
                }
            }

            return builder.ToString();
        }

        private string GetSlot(string slot)
        {
            if (slot == null)
                return string.Empty;
            Match match = Regex.Match(slot, @"\[Slot (\d)\]");
            if (match == null)
                return string.Empty;
            if (!match.Success)
                return string.Empty;
            if (match.Groups.Count < 2)
                return string.Empty;
            if (match.Groups[0].Captures.Count < 1)
                return string.Empty;

            return match.Groups[1].Captures[0].Value;
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