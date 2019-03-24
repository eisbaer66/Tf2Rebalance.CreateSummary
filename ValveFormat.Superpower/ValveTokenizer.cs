using System;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace ValveFormat.Superpower
{
    static class ValveTokenizer
    {
        static TextParser<Unit> ValveStringToken { get; } =
            from open in Character.EqualTo('"')
            from content in Span.EqualTo("\\\"").Value(Unit.Value).Try()
                .Or(Span.EqualTo("\\\\").Value(Unit.Value).Try())
                .Or(Character.Except('"').Value(Unit.Value))
                .IgnoreMany()
            from close in Character.EqualTo('"')
            select Unit.Value;

        private static TextParser<Unit> LineCommentToken { get; } =
            from begin in Span.EqualTo("//")
            from content in Character.ExceptIn('\r', '\n').IgnoreMany()
            select Unit.Value;

        public static Tokenizer<ValveToken> Instance { get; } =
            new TokenizerBuilder<ValveToken>()
                .Ignore(Span.WhiteSpace)
                .Ignore(LineCommentToken)
                .Match(Character.EqualTo('{'), ValveToken.LBracket)
                .Match(Character.EqualTo('}'), ValveToken.RBracket)
                .Match(ValveStringToken, ValveToken.String)
                .Build();
    }
}
