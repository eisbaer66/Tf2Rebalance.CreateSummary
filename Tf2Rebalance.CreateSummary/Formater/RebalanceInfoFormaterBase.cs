using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Tf2Rebalance.CreateSummary
{
    public class Attribute
    {
        public string id      { get; set; }
        public string value      { get; set; }
    }
    public class Weapon
    {
        public string id      { get; set; }
        public string name      { get; set; }
    }
    public class Info
    {
        public string info      { get; set; }
        public string category  { get; set; }
        public string itemclass { get; set; }
        public string slot      { get; set; }

        public IEnumerable<Weapon> weapons { get; set; }

        public IEnumerable<Attribute> attributes { get; set; }
    }

    public class Slot
    {
        public string name { get; set; }
        public IEnumerable<Info> infos { get; set; }
    }

    public class Class
    {
        public string name { get; set; }
        public IEnumerable<Slot> slots { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
        public IEnumerable<Class> classes { get; set; }
    }

    public abstract class RebalanceInfoFormaterBase : IRebalanceInfoFormater
    {
        protected string SlotPattern = @"\[Slot (\d)\]";

        public string Create(IEnumerable<RebalanceInfo> infos)
        {
            var groupings = infos
                .OrderBy(c => c.name)
                .GroupBy(x => new { x.category, x.itemclass, x.slot })
                .OrderBy(s => GetSlot(s.Key.slot))
                .GroupBy(x => new { x.Key.category, x.Key.itemclass })
                .OrderBy(c => c.Key.itemclass)
                .GroupBy(x => x.Key.category)
                .OrderBy(c => c.Key)
                .Select(classes => new Category
                                   {
                                       name = classes.Key, 
                                       classes = classes.Select(slots => new Class
                                                                         {
                                                                             name = slots.Key.itemclass,
                                                                             slots = slots.Select(weapons => new Slot
                                                                                                              {
                                                                                                                  name = weapons.Key.slot,
                                                                                                                  infos = weapons.GroupBy(x => x.info)
                                                                                                                                 .Select(g =>
                                                                                                                                         {
                                                                                                                                             var    itemInfo = g.First();
                                                                                                                                             return new Info
                                                                                                                                             {
                                                                                                                                                 info = g.Key,
                                                                                                                                                 category = itemInfo.category,
                                                                                                                                                 itemclass = itemInfo.itemclass,
                                                                                                                                                 slot = itemInfo.slot,
                                                                                                                                                 weapons = g.Select(w => new Weapon
                                                                                                                                                                         {
                                                                                                                                                                             id = w.id,
                                                                                                                                                                             name = w.name,
                                                                                                                                                                         }),
                                                                                                                                                 attributes = itemInfo.attributes.Select(a => new Attribute
                                                                                                                                                                                              {
                                                                                                                                                                                                  id = a.id,
                                                                                                                                                                                                  value = a.value,
                                                                                                                                                                                              })
                                                                                                                                             };
                                                                                                                                         })
                                                                             })
                                                                         })
                                   });

            Init();
            Process(groupings);

            return Finalize();
        }

        protected abstract void Init();
        protected abstract void Process(IEnumerable<Category> groupings);
        protected abstract string Finalize();

        private string GetSlot(string slot)
        {
            if (slot == null)
                return string.Empty;
            Match match = Regex.Match(slot, SlotPattern);
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
    }
}