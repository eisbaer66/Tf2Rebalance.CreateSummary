﻿using Superpower;
using Superpower.Parsers;

namespace ValveFormat.Superpower
{
    static class ValveTextParsers
    {
        public static TextParser<string> String { get; } =
            from open in Character.EqualTo('"')
            from chars in Character.ExceptIn('"', '\\')
                .Or(Character.EqualTo('\\')
                    .IgnoreThen(
                        Character.EqualTo('\\')
                            .Or(Character.EqualTo('"'))
                            .Or(Character.EqualTo('/'))
                            .Or(Character.EqualTo('b').Value('\b'))
                            .Or(Character.EqualTo('f').Value('\f'))
                            .Or(Character.EqualTo('n').Value('\n'))
                            .Or(Character.EqualTo('r').Value('\r'))
                            .Or(Character.EqualTo('t').Value('\t'))
                            .Or(Character.EqualTo('u').IgnoreThen(
                                Span.MatchedBy(Character.HexDigit.Repeat(4))
                                    .Apply(Numerics.HexDigitsUInt32)
                                    .Select(cc => (char) cc)))
                            .Named("escape sequence")))
                .Many()
            from close in Character.EqualTo('"')
            select new string(chars);
    }
}