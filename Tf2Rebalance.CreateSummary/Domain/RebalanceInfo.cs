using System.Collections.Generic;

namespace Tf2Rebalance.CreateSummary.Domain
{
    public class RebalanceInfo
    {
        public string                          id               { get; set; }
        public string                          name             { get; set; }
        public string                          info             { get; set; }
        public string                          category         { get; set; }
        public string                          itemclass        { get; set; }
        public string                          slot             { get; set; }
        public IEnumerable<RebalanceAttribute> attributes       { get; set; }
        public Dictionary<string, string>      additionalFields { get; set; }
    }
}