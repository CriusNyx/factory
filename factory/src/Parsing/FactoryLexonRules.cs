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
