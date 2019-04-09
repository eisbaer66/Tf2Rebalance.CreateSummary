using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using Serilog;

namespace Tf2Rebalance.CreateSummary
{
    public class AlliedModsWiki
    {
        private static ILogger Log = Serilog.Log.ForContext<AlliedModsWiki>();

        public static IDictionary<string, List<ItemInfo>> GetItemInfos()
        {
            string url = "https://wiki.alliedmods.net/Team_Fortress_2_Item_Definition_Indexes";

            Log.Information("loading weaponnames from {WeaponNameDownloadUrl}", url);

            WebClient client = new WebClient();
            string html = client.DownloadString(url);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            var rows = document.DocumentNode.SelectNodes("//table")
                .SelectMany(table =>
                {
                    HtmlNode slotNode = FindPrevious(table.PreviousSibling, "h4", "h3");
                    HtmlNode classNode = FindPrevious((slotNode?? table).PreviousSibling, "h3", "h2");
                    HtmlNode categoryNode = FindPrevious((classNode?? slotNode ?? table).PreviousSibling, "h2");
                    IEnumerable<HtmlNode> rowNodes = table.SelectNodes("./tr").Skip(1);
                    return rowNodes.Select(row => new {
                        categoryNode,
                        classNode,
                        slotNode,
                        row,
                    });
                })
                .Select(d =>
                {
                    var data = d.row.SelectNodes("./th|./td");
                    if (data.Count < 2)
                        return null;

                    string id = data[0].InnerText.Trim();
                    string name = data[1].InnerText.Trim();
                    return new
                    {
                        id,
                        name,
                        slot = d.slotNode?.InnerText ?? String.Empty,
                        itemclass = d.classNode?.InnerText ?? String.Empty,
                        category = d.categoryNode?.InnerText ?? String.Empty,
                    };
                })
                .ToLookup(x => x.id, x => new ItemInfo
                {
                    Id = x.id,
                    Name = x.name,
                    Slot = x.slot,
                    Class = x.itemclass,
                    Category = x.category,
                })
                .ToDictionary(x => x.Key, x => x.ToList());

            Log.Information("{WeaponNameCount} weaponnames found", rows.Count);
            return rows;
        }

        private static HtmlNode FindPrevious(HtmlNode node, string tag, string cancleTag = null)
        {
            if (cancleTag != null && node.Name == cancleTag)
                return null;
            if (node.Name == tag)
                return node;

            if (node.PreviousSibling == null)
                return null;

            return FindPrevious(node.PreviousSibling, tag, cancleTag);
        }
    }

    public class ItemInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slot { get; set; }
        public string Class { get; set; }
        public string Category { get; set; }
    }
}