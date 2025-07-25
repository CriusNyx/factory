using System.ComponentModel.Design;
using FactorySpracheParser;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace SuperpowerParser;

public static class SuperpowerTokenizer
{
  public static readonly Superpower.Tokenizer<SpracheTokenType> tokenizer =
    new TokenizerBuilder<SpracheTokenType>()
      // Non Semantic
      .Ignore(Span.WhiteSpace)
      .Ignore(Comment.CPlusPlusStyle)
      // Language Symbols
      .Match(Span.EqualTo("..."), SpracheTokenType.spread)
      .Match(Character.EqualTo('.'), SpracheTokenType.dot)
      .Match(Character.EqualTo(','), SpracheTokenType.comma)
      .Match(Character.EqualTo('('), SpracheTokenType.openParen)
      .Match(Character.EqualTo(')'), SpracheTokenType.closedParen)
      .Match(Character.EqualTo('='), SpracheTokenType.equalSign)
      .Match(Character.EqualTo(';'), SpracheTokenType.semicolon)
      // Math
      .Match(Character.EqualTo('+'), SpracheTokenType.plus)
      .Match(Character.EqualTo('-'), SpracheTokenType.minus)
      .Match(Character.EqualTo('*'), SpracheTokenType.asterisk)
      .Match(Character.EqualTo('/'), SpracheTokenType.forwardSlash)
      .Match(Character.EqualTo('%'), SpracheTokenType.percent)
      // Keywords
      .Match(Span.EqualTo("inline").Keyword(), SpracheTokenType.inlineKeyword)
      .Match(Span.EqualTo("line").Keyword(), SpracheTokenType.lineKeyword)
      .Match(Span.EqualTo("out").Keyword(), SpracheTokenType.outKeyword)
      .Match(Span.EqualTo("in").Keyword(), SpracheTokenType.inKeyword)
      .Match(Span.EqualTo("alt").Keyword(), SpracheTokenType.altKeyword)
      .Match(Span.EqualTo("print").Keyword(), SpracheTokenType.printKeyword)
      .Match(Span.EqualTo("tally").Keyword(), SpracheTokenType.tallyKeyword)
      .Match(Span.EqualTo("limit").Keyword(), SpracheTokenType.limitKeyword)
      // Literals
      .Match(QuotedString.CStyle, SpracheTokenType.stringLiteral)
      .Match(Span.Regex("[+-]?([0-9]*[.])?[0-9]+"), SpracheTokenType.numberLiteral)
      // Symbol
      .Match(Identifier.CStyle, SpracheTokenType.symbol)
      .Build();

  public static TokenList<SpracheTokenType> Tokenize(string source)
  {
    return tokenizer.Tokenize(source);
  }

  public static TextParser<T> ThenIgnore<T, U>(this TextParser<T> parser, TextParser<U> ignored)
  {
    return from parsed in parser from ignore in ignored select parsed;
  }

  public static TextParser<T> Keyword<T>(this TextParser<T> parser)
  {
    return parser.ThenIgnore(
      Parse.Not(Parse.OneOf(Character.LetterOrDigit, Character.EqualTo('_'))).Try()
    );
  }
}
