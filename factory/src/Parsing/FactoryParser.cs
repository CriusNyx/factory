using GenParse.Grammar;
using GenParse.Parsing;

namespace Factory.Parsing;

public static class FactoryParser
{
  public static Parser<FactoryLexon> GenerateFactoryParser()
  {
    var grammar = GrammarParser.ParseGrammar(
      [
        File.ReadAllText(
          Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Grammar/factory.grammar")
        ),
      ],
      FactoryLexonRules.lexonFromName
    );
    return new Parser<FactoryLexon>(grammar);
  }
}
