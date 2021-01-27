using System;
using System.Collections.Generic;
using System.Linq;
using Tf2Rebalance.CreateSummary.Domain;
using ValveFormat.Superpower;

namespace Tf2Rebalance.CreateSummary.Converters.Transformations
{
    public class CustomAttributesTransformation : ITransformation<Node>
    {
        private readonly IItemInfoSource  _itemInfoSource;
        private readonly IClassNameSource _classNameSource;

        public CustomAttributesTransformation(IItemInfoSource itemInfoSource, IClassNameSource classNameSource)
        {
            _itemInfoSource  = itemInfoSource  ?? throw new ArgumentNullException(nameof(itemInfoSource));
            _classNameSource = classNameSource ?? throw new ArgumentNullException(nameof(classNameSource));
        }

        public IEnumerable<RebalanceInfo> Transform(IList<Node> definitionNodes)
        {
            var weaponNode = definitionNodes.SelectMany(n => n.Childs);

            return weaponNode.SelectMany(w =>
                                  {
                                      var weaponIds = w.Name.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                                       .Select(s => s.Trim());
                                      return weaponIds.SelectMany(id => _itemInfoSource.Get(id))
                                                      .Select(info =>
                                                              {

                                                                  return new RebalanceInfo
                                                                         {
                                                                             id        = info.Id,
                                                                             name      = info.Name,
                                                                             info      = "from Custom Attributes",
                                                                             category  = info.Category,
                                                                             itemclass = _classNameSource.NormalizeClassName(info.Class),
                                                                             slot      = info.Slot,
                                                                             attributes = w.Childs
                                                                                           .Select(a => new RebalanceAttribute
                                                                                                        {
                                                                                                            id    = a.Name,
                                                                                                            value = a.Value,
                                                                                                        }),
                                                                             additionalFields = new Dictionary<string, string>(),
                                                                         };
                                                              });
                                  });
        }
    }
}
