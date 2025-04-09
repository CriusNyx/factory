using SharpParse.Lexing;

namespace Factory;

public class FactoryLexer
{
  public static Lexon<FactoryLexon>[] LexFactory(string code, bool resumeAfterError = false)
  {
    if (resumeAfterError)
    {
      return LexFactoryWithErrors(code);
    }
    return LexerStatic.Lex(
      code,
      FactoryLexonRules.Rules,
      (lexonType, source, index) =>
        new Lexon<FactoryLexon>(
          lexonType,
          source,
          !FactoryLexonRules.nonSemanticLexon.Contains(lexonType),
          index
        )
    );
  }

  private static Lexon<FactoryLexon>[] LexFactoryWithErrors(string code)
  {
    List<Lexon<FactoryLexon>> list = new List<Lexon<FactoryLexon>>();
    int index = 0;
    while (index < code.Length)
    {
      var lexons = LexerStatic.Lex(code, FactoryLexonRules.Rules, CreateLexon, index);
      list.AddRange(lexons);
      var last = lexons.LastOrDefault();
      index = Math.Max(last?.end ?? 0 + 1, index + 1);
    }
    return list.ToArray();
  }

  private static Lexon<FactoryLexon> CreateLexon(FactoryLexon lexonType, string source, int index)
  {
    return new Lexon<FactoryLexon>(
      lexonType,
      source,
      !FactoryLexonRules.nonSemanticLexon.Contains(lexonType),
      index
    );
  }
}
