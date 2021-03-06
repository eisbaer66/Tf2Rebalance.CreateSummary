﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Tf2Rebalance.CreateSummary.Domain;

namespace Tf2Rebalance.CreateSummary.Formatter
{
    public class RebalanceInfoJsonFormatter : IRebalanceInfoFormatter
    {
        public string Create(IEnumerable<RebalanceInfo> infos)
        {
            return JsonConvert.SerializeObject(infos, Formatting.Indented, new JsonSerializerSettings
                                                                           {
                                                                               NullValueHandling = NullValueHandling.Ignore,
                                                                           });
        }

        public string FileExtension => "json";
    }
}