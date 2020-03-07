using System.Collections.Generic;

namespace Tf2Rebalance.CreateSummary.Formatter
{
    public interface IRebalanceInfoFormatter
    {
        string Create(IEnumerable<RebalanceInfo> infos);
        string FileExtension { get; }
    }
}