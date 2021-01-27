using System;
using System.Collections.Generic;
using Tf2Rebalance.CreateSummary.Converters.Parsers;
using Tf2Rebalance.CreateSummary.Converters.Transformations;
using Tf2Rebalance.CreateSummary.Domain;

namespace Tf2Rebalance.CreateSummary.Converters
{
    public class GenericConverter<T> : IConverter
    {
        private readonly IParser<T>                       _parser;
        private readonly ITransformation<T>               _transformation;

        public GenericConverter(IParser<T> parser, ITransformation<T> transformation)
        {
            _parser         = parser         ?? throw new ArgumentNullException(nameof(parser));
            _transformation = transformation ?? throw new ArgumentNullException(nameof(transformation));
        }

        public IEnumerable<RebalanceInfo> Execute(string input)
        {
            var nodes = _parser.Parse(input);
            return _transformation.Transform(nodes);
        }
    }
}