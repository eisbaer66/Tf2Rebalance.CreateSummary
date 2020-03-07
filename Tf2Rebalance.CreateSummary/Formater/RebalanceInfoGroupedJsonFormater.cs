using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Tf2Rebalance.CreateSummary
{
    public class RebalanceInfoGroupedJsonFormater : RebalanceInfoFormaterBase
    {
        private string _output;

        protected override void Init()
        {
            
        }

        protected override void Process(IDictionary<string, Category> groupings)
        {
            var obj = groupings;
            _output = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
                                                                            {
                                                                                NullValueHandling = NullValueHandling.Ignore,
                                                                            });
        }

        protected override string Finalize()
        {
            return _output;
        }
    }
}