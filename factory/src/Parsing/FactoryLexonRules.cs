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
  comma,
  openParen,
  closeParen,

  // keywords
  recipeKeyword,
  altKeyword,
  outKeyword,
  inKeyword,
  printKeyword,
  tallyKeyword,

  // Literals
  numberLiteral,

  // Identifier Symbols
  symbol,

  // None
  None
}



internal static class FactoryLexonRules
{
  public static readonly FactoryLexon[] nonSemanticLexon = [FactoryLexon.whitespace, FactoryLexon.comment];

  static private readonly (FactoryLexon lexonType, string expression)[] rules = [
    // non semantic
    (FactoryLexon.comment, @"^//.*\n"),
    (FactoryLexon.whitespace, @"^\s+"),

    // Language Symbols
    (FactoryLexon.comma, @"^,"),
    (FactoryLexon.openParen, @"^\("),
    (FactoryLexon.closeParen, @"^\)"),

    // keywords
    (FactoryLexon.recipeKeyword, @"^recipe"),
    (FactoryLexon.altKeyword, @"^alt"),
    (FactoryLexon.outKeyword, @"^out"),
    (FactoryLexon.inKeyword, @"^in"), 
    (FactoryLexon.printKeyword, @"^print"),
    (FactoryLexon.tallyKeyword, @"^tally"),

    // Literals
    (FactoryLexon.numberLiteral, @"^[+-]?([0-9]*[.])?[0-9]+"),

    // Identifier Symbols
    (FactoryLexon.symbol, @"^\p{L}\w*"),
  ];

  static (FactoryLexon lexonType, Regex regex) GenerateRule((FactoryLexon lexonType, string expression) value)
  {
    return (value.lexonType, new Regex(value.expression));
  }

  static public readonly (FactoryLexon lexonType, Regex regex)[] Rules =
    new Thunk<(FactoryLexon, Regex)[]>(() => rules.Map(GenerateRule));

  public static FactoryLexon lexonFromName(string name){
    if(Enum.TryParse<FactoryLexon>(name, out var result)){
      return result;
    }
    return FactoryLexon.None;
  }
}
