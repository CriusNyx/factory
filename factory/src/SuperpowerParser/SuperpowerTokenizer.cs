using System.ComponentModel.Design;
using FactorySpracheParser;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace SuperpowerParser;

public static class SuperpowerTokenizer
{
  public static readonly Superpower.Tokenizer<SuperpowerTokenType> tokenizer =
    new TokenizerBuilder<SuperpowerTokenType>()
      // Non Semantic
      .Ignore(Span.WhiteSpace)
      .Ignore(Comment.CPlusPlusStyle)
      // Language Symbols
      .Match(Span.EqualTo("..."), SuperpowerTokenType.spread)
      .Match(Character.EqualTo('.'), SuperpowerTokenType.dot)
      .Match(Character.EqualTo(','), SuperpowerTokenType.comma)
      .Match(Character.EqualTo('('), SuperpowerTokenType.openParen)
      .Match(Character.EqualTo(')'), SuperpowerTokenType.closedParen)
      .Match(Character.EqualTo('='), SuperpowerTokenType.equalSign)
      .Match(Character.EqualTo(';'), SuperpowerTokenType.semicolon)
      // Math
      .Match(Character.EqualTo('+'), SuperpowerTokenType.plus)
      .Match(Character.EqualTo('-'), SuperpowerTokenType.minus)
      .Match(Character.EqualTo('*'), SuperpowerTokenType.asterisk)
      .Match(Character.EqualTo('/'), SuperpowerTokenType.forwardSlash)
      .Match(Character.EqualTo('%'), SuperpowerTokenType.percent)
      // Keywords
      .Match(Span.EqualTo("inline").Keyword(), SuperpowerTokenType.inlineKeyword)
      .Match(Span.EqualTo("line").Keyword(), SuperpowerTokenType.lineKeyword)
      .Match(Span.EqualTo("out").Keyword(), SuperpowerTokenType.outKeyword)
      .Match(Span.EqualTo("in").Keyword(), SuperpowerTokenType.inKeyword)
      .Match(Span.EqualTo("alt").Keyword(), SuperpowerTokenType.altKeyword)
      .Match(Span.EqualTo("print").Keyword(), SuperpowerTokenType.printKeyword)
      .Match(Span.EqualTo("tally").Keyword(), SuperpowerTokenType.tallyKeyword)
      .Match(Span.EqualTo("limit").Keyword(), SuperpowerTokenType.limitKeyword)
      // Literals
      .Match(QuotedString.CStyle, SuperpowerTokenType.stringLiteral)
      .Match(Span.Regex("[+-]?([0-9]*[.])?[0-9]+"), SuperpowerTokenType.numberLiteral)
      // Symbol
      .Match(Identifier.CStyle, SuperpowerTokenType.symbol)
      .Build();

  public static TokenList<SuperpowerTokenType> Tokenize(string source)
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
