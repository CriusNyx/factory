using System.Text.RegularExpressions;
using GenParse.Functional;
using GenParse.Util;

namespace Factory.Parsing;

public enum FactoryLexon
{
  // Non semantic groups
  comment,
  whitespace,

  // Language Symbols
  dot,
  comma,
  openParen,
  closeParen,
  equalSign,
  spread,

  // keywords
  letKeyword,
  recipeKeyword,
  altKeyword,
  outKeyword,
  inKeyword,
  printKeyword,
  tallyKeyword,
  inlineKeyword,
  limitKeyword,

  // Literals
  numberLiteral,

  // Identifier Symbols
  symbol,

  // None
  None,
}

internal static class FactoryLexonRules
{
  public static readonly FactoryLexon[] nonSemanticLexon =
  [
    FactoryLexon.whitespace,
    FactoryLexon.comment,
  ];

  private static readonly (FactoryLexon lexonType, string expression)[] rules =
  [
    // non semantic
    (FactoryLexon.comment, @"^//.*\n"),
    (FactoryLexon.whitespace, @"^\s+"),
    // Operators


    // Language Symbols
    (FactoryLexon.spread, @"^\.\.\."),
    (FactoryLexon.dot, @"^\."),
    (FactoryLexon.comma, @"^,"),
    (FactoryLexon.openParen, @"^\("),
    (FactoryLexon.closeParen, @"^\)"),
    (FactoryLexon.equalSign, @"^\="),
    // keywords
    (FactoryLexon.letKeyword, @"^let"),
    (FactoryLexon.recipeKeyword, @"^recipe"),
    (FactoryLexon.altKeyword, @"^alt"),
    (FactoryLexon.outKeyword, @"^out"),
    (FactoryLexon.printKeyword, @"^print"),
    (FactoryLexon.tallyKeyword, @"^tally"),
    (FactoryLexon.inlineKeyword, @"^inline"),
    (FactoryLexon.inKeyword, @"^in"),
    (FactoryLexon.limitKeyword, @"^limit"),
    // Literals
    (FactoryLexon.numberLiteral, @"^[+-]?([0-9]*[.])?[0-9]+"),
    // Identifier Symbols
    (FactoryLexon.symbol, @"^\p{L}\w*"),
  ];

  static (FactoryLexon lexonType, Regex regex) GenerateRule(
    (FactoryLexon lexonType, string expression) value
  )
  {
    return (value.lexonType, new Regex(value.expression));
  }

  public static readonly (FactoryLexon lexonType, Regex regex)[] Rules = new Thunk<(
    FactoryLexon,
    Regex
  )[]>(() => rules.Map(GenerateRule));

  public static FactoryLexon lexonFromName(string name)
  {
    if (Enum.TryParse<FactoryLexon>(name, out var result))
    {
      return result;
    }
    return FactoryLexon.None;
  }
}
