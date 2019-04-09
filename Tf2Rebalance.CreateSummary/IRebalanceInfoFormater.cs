using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tf2Rebalance.CreateSummary
{
    public interface IRebalanceInfoFormater
    {
        string Create(IEnumerable<RebalanceInfo> infos);
    }

    public abstract class RebalanceInfoFormaterBase : IRebalanceInfoFormater
    {
        public string Create(IEnumerable<RebalanceInfo> infos)
        {
            var groupings = infos
                .GroupBy(x => new { x.category, x.itemclass, x.slot })
                .OrderBy(s => GetSlot(s.Key.slot))
                .GroupBy(x => new { x.Key.category, x.Key.itemclass })
                .GroupBy(x => x.Key.category);

            Init();
            foreach (var classes in groupings.OrderBy(c => c.Key))
            {
                WriteCategory(classes.Key);
                foreach (var slots in classes.OrderBy(c => c.Key.itemclass))
                {
                    string itemclass = slots.Key.itemclass;
                    if (!string.IsNullOrEmpty(itemclass))
                        WriteClass(itemclass);

                    foreach (var weapons in slots.OrderBy(c => c.Key.slot))
                    {
                        string slot = weapons.Key.slot;
                        if (!string.IsNullOrEmpty(slot))
                            WriteSlot(slot);

                        var groupedWeapons = weapons.GroupBy(x => x.info)
                            .Select(g =>
                            {
                                string name = string.Join(", ", g.Select(i => i.name));
                                var itemInfo = g.First();
                                itemInfo.name = name;
                                return itemInfo;
                            });
                        foreach (var weapon in groupedWeapons.OrderBy(c => c.name))
                        {
                            Write(weapon);
                        }
                    }
                }
            }

            return Finalize();
        }
        
        protected abstract void Init();
        protected abstract void WriteCategory(string text);
        protected abstract void WriteClass(string text);
        protected abstract void WriteSlot(string text);
        protected abstract void Write(RebalanceInfo weapon);
        protected abstract string Finalize();

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
    }

    public class RebalanceInfoTextFormater : RebalanceInfoFormaterBase
    {
        private StringBuilder _builder;

        protected override void Init()
        {
            _builder = new StringBuilder();
        }

        protected override void WriteCategory(string text)
        {
            _builder.AppendLine(text);
        }

        protected override void WriteClass(string text)
        {
            _builder.AppendLine(text);
        }

        protected override void WriteSlot(string text)
        {
            _builder.AppendLine(text);
        }

        protected override void Write(RebalanceInfo weapon)
        {
            _builder.AppendLine(weapon.name);
            _builder.AppendLine(weapon.info);
            _builder.AppendLine();
        }

        protected override string Finalize()
        {
            return _builder.ToString();
        }
    }
}