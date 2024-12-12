using GenParse.Lexing;

namespace Factory.Parsing;

public class FactoryLexer
{
  public static Lexon<FactoryLexon>[] LexFactory(string code)
  {
    return Lexer.Lex(
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
}
