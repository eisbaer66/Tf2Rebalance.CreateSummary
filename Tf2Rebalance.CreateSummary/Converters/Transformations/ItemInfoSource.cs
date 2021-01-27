using System;
using System.Collections.Generic;

namespace Tf2Rebalance.CreateSummary.Converters.Transformations
{
    public interface IItemInfoSource
    {
        IList<ItemInfo> Get(string id);
    }

    public class ItemInfoSource : IItemInfoSource
    {
        private readonly IDictionary<string, List<ItemInfo>> _itemInfos;

        public ItemInfoSource(IDictionary<string, List<ItemInfo>> itemInfos)
        {
            _itemInfos = itemInfos ?? throw new ArgumentNullException(nameof(itemInfos));
        }

        public IList<ItemInfo> Get(string id)
        {
            if (!_itemInfos.ContainsKey(id))
                return new List<ItemInfo>
                       {
                           new ItemInfo
                           {
                               Name     = id,
                               Id       = id,
                               Category = "unknown",
                           }
                       };

            return _itemInfos[id];
        }
    }
}