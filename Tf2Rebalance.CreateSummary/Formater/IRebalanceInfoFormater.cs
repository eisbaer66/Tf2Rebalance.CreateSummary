using System;
using System.Collections.Generic;

namespace Tf2Rebalance.CreateSummary
{
    public interface IRebalanceInfoFormater
    {
        string Create(IEnumerable<RebalanceInfo> infos);
    }
}