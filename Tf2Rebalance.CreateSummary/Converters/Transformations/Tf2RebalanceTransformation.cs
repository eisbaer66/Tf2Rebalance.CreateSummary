using System;
using System.Collections.Generic;
using System.Linq;
using Tf2Rebalance.CreateSummary.Domain;
using ValveFormat.Superpower;

namespace Tf2Rebalance.CreateSummary.Converters.Transformations
{
    public class Tf2RebalanceTransformation : ITransformation<Node>
    {
        private readonly IItemInfoSource  _itemInfoSource;
        private readonly IClassNameSource _classNameSource;

        public Tf2RebalanceTransformation(IItemInfoSource itemInfoSource, IClassNameSource classNameSource)
        {
            _itemInfoSource  = itemInfoSource  ?? throw new ArgumentNullException(nameof(itemInfoSource));
            _classNameSource = classNameSource ?? throw new ArgumentNullException(nameof(classNameSource));
        }

        public IEnumerable<RebalanceInfo> Transform(IList<Node> definitionNodes)
        {
            var groupings = FindItemNodes(definitionNodes)
                .SelectMany(node => FindAllInfos(node.Parent.Name)
                                .Select(i =>
                                        {
                                            return new RebalanceInfo
                                                   {
                                                       id = i.Id,
                                                       name = _classNameSource.TryGet(i.Name) ?? i.Name,
                                                       info      = node.Value.Replace("\n", "\r\n"),
                                                       category  = i.Category,
                                                       itemclass = _classNameSource.NormalizeClassName(i.Class),
                                                       slot      = i.Slot,
                                                       attributes = node.Parent
                                                                        .Childs.Where(c => c.Name.ToLower().StartsWith("attribute"))
                                                                        .Select(n => new RebalanceAttribute
                                                                                     {
                                                                                         id    = n.Childs.FirstOrDefault(c => c.Name == "id")?.Value,
                                                                                         value = n.Childs.FirstOrDefault(c => c.Name == "value")?.Value,
                                                                                     }),
                                                       additionalFields = node.Parent
                                                                              .Childs.Where(c =>
                                                                                            {
                                                                                                var name = c.Name.ToLower();
                                                                                                return !name.StartsWith("attribute") &&
                                                                                                       name != "info"                &&
                                                                                                       name != "keepattribs";
                                                                                            })
                                                                              .ToDictionary(n => n.Name, n => n.Value),
                                                   };
                                        }));

            return groupings;
        }

        private IEnumerable<ItemInfo> FindAllInfos(string text)
        {
            return text.Split(';')
                       .Select(s => s.Trim())
                       .SelectMany(id =>
                                   {
                                       var classname = _classNameSource.TryGet(id);
                                       if (classname != null)
                                       {
                                           return new List<ItemInfo>
                                                  {
                                                      new ItemInfo
                                                      {
                                                          Name     = classname,
                                                          Id       = id,
                                                          Category = "Classes"
                                                      }
                                                  };
                                       }

                                       return _itemInfoSource.Get(id);
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