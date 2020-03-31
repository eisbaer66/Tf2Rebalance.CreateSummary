using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tf2Rebalance.CreateSummary.Tests
{
    [TestClass]
    public class AlliedModsWikiTests
    {
        [TestMethod]
        public void AtLeast1968Items()
        {
            IDictionary<string, List<ItemInfo>> infos = AlliedModsWiki.GetItemInfos();

            Assert.IsTrue(infos.Count >= 1968);
        }

    }
}