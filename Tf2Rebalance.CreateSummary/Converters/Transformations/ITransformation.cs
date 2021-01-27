using System.Collections.Generic;
using Tf2Rebalance.CreateSummary.Domain;

namespace Tf2Rebalance.CreateSummary.Converters.Transformations
{
    public interface ITransformation<T>
    {
        IEnumerable<RebalanceInfo> Transform(IList<T> definitionNodes);
    }
}