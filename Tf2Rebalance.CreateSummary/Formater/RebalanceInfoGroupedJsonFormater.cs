using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tf2Rebalance.CreateSummary
{
    public class RebalanceInfoGroupedJsonFormater : RebalanceInfoFormaterBase
    {
        private string _output;

        protected override void Init()
        {
            
        }

        protected override void Process(IEnumerable<Category> groupings)
        {
            _output = JsonConvert.SerializeObject(groupings, Formatting.Indented);
        }

        protected override string Finalize()
        {
            return _output;
        }
    }
}