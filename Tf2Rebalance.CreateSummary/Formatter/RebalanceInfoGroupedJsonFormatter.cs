using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tf2Rebalance.CreateSummary.Formatter
{
    public class RebalanceInfoGroupedJsonFormatter : RebalanceInfoFormatterBase
    {
        private string _output;

        public override string FileExtension => "json";

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