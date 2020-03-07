using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tf2Rebalance.CreateSummary
{
    public abstract class RebalanceInfoFormaterIterativeBase : RebalanceInfoFormaterBase
    {
        protected override void Process(IDictionary<string, Category> groupings)
        {
            foreach (var category in groupings.Values)
            {
                WriteCategory(category.name);
                foreach (var @class in category.classes.Values)
                {
                    string itemclass = @class.name;
                    if (!string.IsNullOrEmpty(itemclass))
                        WriteClass(itemclass);

                    foreach (var slot in @class.slots.Values)
                    {
                        string slotname = slot.name;
                        if (!string.IsNullOrEmpty(slotname))
                        {
                            slotname = Regex.Replace(slotname, SlotPattern, string.Empty);
                            WriteSlot(slotname);
                        }

                        foreach (var info in slot.infos)
                        {
                            string weaponnames = info.weapons.Aggregate(new StringBuilder(), (s, w)=>
                                                                                      {
                                                                                          if (s.Length > 0)
                                                                                              s.Append(", ");
                                                                                          s.Append(w.name);
                                                                                          return s;
                                                                                      }).ToString();
                            Write(weaponnames, info);
                        }
                    }
                }
            }
        }
        protected abstract void WriteCategory(string text);
        protected abstract void WriteClass(string    text);
        protected abstract void WriteSlot(string     text);
        protected abstract void Write(string weaponnames, Info weapon);
    }
}