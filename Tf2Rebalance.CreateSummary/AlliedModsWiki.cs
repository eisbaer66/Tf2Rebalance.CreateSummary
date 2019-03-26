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

        public static IDictionary<string, string> GetWeaponNames()
        {
            string url = "https://wiki.alliedmods.net/Team_Fortress_2_Item_Definition_Indexes";

            Log.Information("loading weaponnames from {WeaponNameDownloadUrl}", url);

            WebClient client = new WebClient();
            string html = client.DownloadString(url);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            var rows = document.DocumentNode.SelectNodes("//table")
                .SelectMany(table => table.SelectNodes("./tr").Skip(1))
                .Select(row =>
                {
                    var data = row.SelectNodes("./th|./td");
                    if (data.Count < 2)
                        return null;

                    string id = data[0].InnerText.Trim();
                    string name = data[1].InnerText.Trim();
                    return new
                    {
                        id,
                        name
                    };
                })
                .ToLookup(x => x.id, x => x.name)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            Log.Information("{WeaponNameCount} weaponnames found", rows.Count);
            return rows;
        }
    }
}