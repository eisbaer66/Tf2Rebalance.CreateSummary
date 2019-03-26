using System.Collections.Generic;
using System.Linq;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace ValveFormat.Superpower
{
    public static class ValveParser
    {
        static TokenListParser<ValveToken, object> ValveString { get; } =
            Token.EqualTo(ValveToken.String)
                .Apply(ValveTextParsers.String)
                .Select(s => (object)s);


        static TokenListParser<ValveToken, object> ValveObject { get; } =
            from begin in Token.EqualTo(ValveToken.LBracket)
            from properties in ValveString
                .Named("property name")
                .Then(name => Parse.Ref(() => ValveValue)
                    .Select(value => new KeyValuePair<string, object>((string)name, value)))
                .Many()
            from end in Token.EqualTo(ValveToken.RBracket)
            select (object)properties.ToDictionary(p => p.Key, p => p.Value);

        static TokenListParser<ValveToken, object> ValveValue { get; } =
            ValveString
                .Or(ValveObject)
                .Named("JSON value");

        static TokenListParser<ValveToken, object> ValveDocument { get; } =
            from properties in ValveString
                .Named("property name")
                .Then(name => Parse.Ref(() => ValveValue)
                    .Select(value => new KeyValuePair<string, object>((string)name, value)))
                .Many()
                .AtEnd()
            select (object)properties.ToDictionary(p => p.Key, p => p.Value);

        public static bool TryParse(string json, out IList<Node> value, out string error, out Position errorPosition)
        {
            var tokens = ValveTokenizer.Instance.TryTokenize(json);
            if (!tokens.HasValue)
            {
                value = null;
                error = tokens.ToString();
                errorPosition = tokens.ErrorPosition;
                return false;
            }

            var parsed = ValveDocument.TryParse(tokens.Value);
            if (!parsed.HasValue)
            {
                value = null;
                error = parsed.ToString();
                errorPosition = parsed.ErrorPosition;
                return false;
            }

            Dictionary<string, object> dict = (Dictionary<string, object>)parsed.Value;
            List<Node> nodes = GetNodes(dict).ToList();

            value = nodes;
            error = null;
            errorPosition = Position.Empty;
            return true;
        }

        private static IEnumerable<Node> GetNodes(Dictionary<string, object> dict)
        {
            foreach (KeyValuePair<string, object> p in dict)
            {
                if (p.Value is string)
                {
                    yield return new Node(p.Key, (string)p.Value);
                    continue;
                }

                Dictionary<string, object> d = (Dictionary<string, object>)p.Value;
                List<Node> childs = GetNodes(d).ToList();

                Node node = new Node(p.Key, childs);

                foreach (Node child in childs)
                {
                    child.Parent = node;
                }
                yield return node;
            }
        }
    }
}