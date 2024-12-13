using Factory.Parsing;
using GenParse.Functional;

namespace Factory;

public static class FactoryAnalytics
{
  public static FactorySemanticToken[] AnalyzeSemanticTokens(string sourceCode)
  {
    var lexons = FactoryLexer.LexFactory(sourceCode);
    return lexons
      .Map(x => (lexon: x, semanticType: x.GetSemanticType()))
      .Filter(x => x.semanticType != FactorySemanticType.whitespace)
      .Map(x => new FactorySemanticToken(
        x.lexon.index,
        x.lexon.length,
        x.semanticType,
        (int)x.lexon.GetSemanticModifier()
      ));
  }

  public static void AnalyzeProgram(string sourceCode)
  {
    var lexons = FactoryLexer.LexFactory(sourceCode);
  }

  public static string[] GetSemanticTypes()
  {
    return Enum.GetValues<FactorySemanticType>().Map(x => x.ToString());
  }

  public static string[] GetSemanticModifiers()
  {
    // The number of valid flags is 1 less then the number of modifiers because None is not a flag.
    int count = Enum.GetValues<FactorySemanticModifier>().Length - 1;
    return Functional.Range(count).Map(x => ((FactorySemanticModifier)(1 << x)).ToString());
  }

  public static void AnalyzeErrors(string sourceCode)
  {
    var parser = FactoryParser.parser;

    var lexons = FactoryLexer.LexFactory(sourceCode);
  }
}
