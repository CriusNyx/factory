using Factory.Parsing;
using GenParse.Lexing;

public static class LexonExtensions
{
  public static FactorySemanticType GetSemanticType(this Lexon<FactoryLexon> lexon)
  {
    switch (lexon.lexonType)
    {
      case FactoryLexon.comment:
        return FactorySemanticType.comment;
      case FactoryLexon.whitespace:
        return FactorySemanticType.whitespace;
      case FactoryLexon.dot:
      case FactoryLexon.comma:
      case FactoryLexon.openParen:
      case FactoryLexon.closeParen:
      case FactoryLexon.equalSign:
      case FactoryLexon.spread:
        return FactorySemanticType.@operator;
      case FactoryLexon.letKeyword:
      case FactoryLexon.recipeKeyword:
      case FactoryLexon.printKeyword:
      case FactoryLexon.altKeyword:
      case FactoryLexon.outKeyword:
      case FactoryLexon.inKeyword:
      case FactoryLexon.tallyKeyword:
      case FactoryLexon.inlineKeyword:
      case FactoryLexon.limitKeyword:
        return FactorySemanticType.keyword;
      case FactoryLexon.numberLiteral:
        return FactorySemanticType.number;
      case FactoryLexon.stringLiteral:
        return FactorySemanticType.@string;
      case FactoryLexon.symbol:
        return FactorySemanticType.variable;
      default:
        throw new NotImplementedException($"Unknown lexon type {lexon.lexonType}");
    }
  }

  public static FactorySemanticModifier GetSemanticModifier(this Lexon<FactoryLexon> lexon)
  {
    if (lexon.lexonType == FactoryLexon.symbol)
    {
      if (Docs.recipesByProductIdentifier.ContainsKey(lexon.sourceCode))
      {
        return FactorySemanticModifier.@readonly;
      }
    }
    return FactorySemanticModifier.none;
  }
}