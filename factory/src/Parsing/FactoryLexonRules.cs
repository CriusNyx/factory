using System.Text.RegularExpressions;
using SharpParse.Functional;
using SharpParse.Util;

namespace Factory;

public static class FactoryLexon
{
  // Non semantic groups
  public const string comment = "comment";
  public const string whitespace = "whitespace";

  // Language Symbols
  public const string dot = "dot";
  public const string comma = "comma";
  public const string openParen = "openParen";
  public const string closeParen = "closeParen";
  public const string equalSign = "equalSign";
  public const string spread = "spread";

  // Math Symbols
  public const string minus = "minus";
  public const string plus = "plus";
  public const string asterisk = "asterisk";
  public const string forwardSlash = "forwardSlash";
  public const string percent = "percent";

  // keywords
  public const string letKeyword = "letKeyword";
  public const string recipeKeyword = "recipeKeyword";
  public const string altKeyword = "altKeyword";
  public const string outKeyword = "outKeyword";
  public const string inKeyword = "inKeyword";
  public const string printKeyword = "printKeyword";
  public const string tallyKeyword = "tallyKeyword";
  public const string inlineKeyword = "inlineKeyword";
  public const string limitKeyword = "limitKeyword";

  // Literals
  public const string numberLiteral = "numberLiteral";
  public const string stringLiteral = "stringLiteral";

  // Identifier Symbols
  public const string symbol = "symbol";

  // None
  public const string none = "none";
}

internal static class FactoryLexonRules
{
  public static readonly string[] nonSemanticLexonTypes =
  [
    FactoryLexon.whitespace,
    FactoryLexon.comment,
  ];

  private static readonly (string lexonType, string expression)[] rules =
  [
    // non semantic
    (FactoryLexon.comment, @"^//.*(\n|$)"),
    (FactoryLexon.whitespace, @"^\s+"),
    // Operators

    // Language Symbols
    (FactoryLexon.spread, @"^\.\.\."),
    (FactoryLexon.dot, @"^\."),
    (FactoryLexon.comma, @"^,"),
    (FactoryLexon.openParen, @"^\("),
    (FactoryLexon.closeParen, @"^\)"),
    (FactoryLexon.equalSign, @"^\="),
    // Math Symbol
    (FactoryLexon.plus, @"^\+"),
    (FactoryLexon.minus, @"^\-"),
    (FactoryLexon.asterisk, @"^\*"),
    (FactoryLexon.forwardSlash, @"^\/"),
    (FactoryLexon.percent, @"^\%"),
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
    (FactoryLexon.stringLiteral, @"^"".*?"""),
    (FactoryLexon.numberLiteral, @"^[+-]?([0-9]*[.])?[0-9]+"),
    // Identifier Symbols
    (FactoryLexon.symbol, @"^\p{L}\w*"),
  ];

  static (string lexonType, Regex regex) GenerateRule((string lexonType, string expression) value)
  {
    return (value.lexonType, new Regex(value.expression));
  }

  public static readonly (string lexonType, Regex regex)[] Rules = new Thunk<(string, Regex)[]>(
    () => rules.Map(GenerateRule)
  );
}
