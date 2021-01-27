using System.Collections.Generic;
using Serilog;
using Superpower.Model;
using ValveFormat.Superpower;

namespace Tf2Rebalance.CreateSummary.Converters.Parsers
{
    public class ValveFormatParser : IParser<Node>
    {
        private static ILogger Log = Serilog.Log.ForContext<ValveFormatParser>();

        public IList<Node> Parse(string input)
        {
            IList<Node> definitionNodes;
            string      error;
            Position    position;
            bool        successfullParse = ValveParser.TryParse(input, out definitionNodes, out error, out position);
            if (!successfullParse)
            {
                Log.Error("Error parsing input: {Error} at {Position}", error, position);
                return null;
            }
            if (definitionNodes.Count == 0)
            {
                Log.Error("no data found in input");
                return null;
            }

            return definitionNodes;
        }
    }
}