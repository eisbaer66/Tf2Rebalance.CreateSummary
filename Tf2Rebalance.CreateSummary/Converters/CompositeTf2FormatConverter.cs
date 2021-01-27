using System;
using System.Collections.Generic;
using Tf2Rebalance.CreateSummary.Converters.Parsers;
using Tf2Rebalance.CreateSummary.Converters.Transformations;
using Tf2Rebalance.CreateSummary.Domain;
using ValveFormat.Superpower;

namespace Tf2Rebalance.CreateSummary.Converters
{
    public class CompositeTf2FormatConverter : IConverter
    {
        private readonly IParser<Node>                              _parser;
        private readonly IDictionary<string, ITransformation<Node>> _transformations;

        public CompositeTf2FormatConverter(IParser<Node> parser, IDictionary<string, ITransformation<Node>> transformations)
        {
            _parser          = parser          ?? throw new ArgumentNullException(nameof(parser));
            _transformations = transformations ?? throw new ArgumentNullException(nameof(transformations));
        }

        public IEnumerable<RebalanceInfo> Execute(string input)
        {
            var nodes = _parser.Parse(input);
            if (nodes == null)
                throw new InvalidInputException("input could not be read. check Logs for additional infos");

            if (nodes.Count == 0)
                return new RebalanceInfo[0];

            var pluginName = nodes[0].Name;
            if (!_transformations.ContainsKey(pluginName))
                throw new InputNotSupportedException("root-element '" + pluginName + "' is not supported (supported are: " + string.Join(", ", _transformations.Keys) + ")");

            return _transformations[pluginName].Transform(nodes);
        }
    }
}