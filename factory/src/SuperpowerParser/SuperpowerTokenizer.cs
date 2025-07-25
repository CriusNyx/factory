using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Factory.Superpower;

public static class SuperpowerTokenizer
{
  public static readonly Tokenizer<SuperpowerTokenType> tokenizer =
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
      .Match(Span.EqualTo("let").Keyword(), SuperpowerTokenType.letKeyword)
      // Literals
      .Match(QuotedString.CStyle, SuperpowerTokenType.stringLiteral)
      .Match(Span.Regex("[+-]?([0-9]*[.])?[0-9]+"), SuperpowerTokenType.numberLiteral)
      // Symbol
      .Match(Identifier.CStyle, SuperpowerTokenType.symbol)
      .Match(Character.AnyChar, SuperpowerTokenType.unknown)
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

public static class SuperpowerTokenizerExtensions
{
  public static FactorySemanticType GetSemanticType(this Token<SuperpowerTokenType> token)
  {
    return token.Kind.GetSemanticType();
  }

  public static FactorySemanticType GetSemanticType(this SuperpowerTokenType type)
  {
    switch (type)
    {
      // Whitespace
      case SuperpowerTokenType.whitespace:
        return FactorySemanticType.whitespace;

      case SuperpowerTokenType.comment:
        return FactorySemanticType.comment;

      // Language Symbols
      case SuperpowerTokenType.spread:
      case SuperpowerTokenType.dot:
      case SuperpowerTokenType.comma:
      case SuperpowerTokenType.openParen:
      case SuperpowerTokenType.closedParen:
      case SuperpowerTokenType.equalSign:
      case SuperpowerTokenType.semicolon:

      // Language Operators
      case SuperpowerTokenType.plus:
      case SuperpowerTokenType.minus:
      case SuperpowerTokenType.asterisk:
      case SuperpowerTokenType.forwardSlash:
      case SuperpowerTokenType.percent:
        return FactorySemanticType.@operator;

      // Language Keywords
      case SuperpowerTokenType.lineKeyword:
      case SuperpowerTokenType.altKeyword:
      case SuperpowerTokenType.outKeyword:
      case SuperpowerTokenType.printKeyword:
      case SuperpowerTokenType.tallyKeyword:
      case SuperpowerTokenType.inlineKeyword:
      case SuperpowerTokenType.inKeyword:
      case SuperpowerTokenType.limitKeyword:
      case SuperpowerTokenType.letKeyword:
        return FactorySemanticType.keyword;

      // String
      case SuperpowerTokenType.stringLiteral:
        return FactorySemanticType.@string;

      // Number
      case SuperpowerTokenType.numberLiteral:
        return FactorySemanticType.number;

      // Symbol
      case SuperpowerTokenType.symbol:
        return FactorySemanticType.variable;

      case SuperpowerTokenType.unknown:
        return FactorySemanticType.whitespace;

      default:
        throw new NotImplementedException();
    }
  }

  public static FactorySemanticModifier GetSemanticModifier(this Token<SuperpowerTokenType> token)
  {
    if (token.Kind == SuperpowerTokenType.symbol)
    {
      if (FactoryLanguage.ResolveGlobal(token.ToStringValue()) != null)
      {
        return FactorySemanticModifier.@readonly;
      }
    }
    return FactorySemanticModifier.none;
  }

  public static bool HasIndex(this Token<SuperpowerTokenType> token, int index)
  {
    return index >= token.Position.Absolute && index <= token.Position.Absolute + token.Span.Length;
  }
}
