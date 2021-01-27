using System.Collections.Generic;

namespace Tf2Rebalance.CreateSummary.Converters.Parsers
{
    public interface IParser<T>
    {
        IList<T> Parse(string input);
    }
}