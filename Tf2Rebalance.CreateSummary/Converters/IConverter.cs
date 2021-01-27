using System.Collections.Generic;
using Tf2Rebalance.CreateSummary.Domain;

namespace Tf2Rebalance.CreateSummary.Converters
{
    public interface IConverter
    {
        IEnumerable<RebalanceInfo> Execute(string   input);
    }
}