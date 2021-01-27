using System.Collections.Generic;
using Tf2Rebalance.CreateSummary.Domain;

namespace Tf2Rebalance.CreateSummary.Formatter
{
    public interface IRebalanceInfoFormatter
    {
        string Create(IEnumerable<RebalanceInfo> infos);
        string FileExtension { get; }
    }
}