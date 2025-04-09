using SharpParse.Lexing;

namespace Factory;

public class FactoryLexer
{
  public static Lexon[] LexFactory(string code, bool resumeAfterError = false)
  {
    if (resumeAfterError)
    {
      return LexFactoryWithErrors(code);
    }
    return LexerStatic.Lex(
      code,
      FactoryLexonRules.Rules,
      (lexonType, source, index) =>
        new Lexon(
          lexonType,
          source,
          !FactoryLexonRules.nonSemanticLexonTypes.Contains(lexonType),
          index
        )
    );
  }

  private static Lexon[] LexFactoryWithErrors(string code)
  {
    List<Lexon> list = new List<Lexon>();
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

  private static Lexon CreateLexon(string lexonType, string source, int index)
  {
    return new Lexon(
      lexonType,
      source,
      !FactoryLexonRules.nonSemanticLexonTypes.Contains(lexonType),
      index
    );
  }
}
