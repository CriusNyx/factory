using SharpParse.Functional;
using SharpParse.Grammar;
using SharpParse.Lexing;
using SharpParse.Parsing;

namespace Factory;

public static class FactoryParser
{
  public static string GrammarSource { get; private set; }

  public static readonly Parser<FactoryLexon> parser;

  public static FactoryLexon[] ProgramLexonHeads => parser.ComputeHead("Program");
  public static FactoryLexon[] RecipeExpLexonHeads => parser.ComputeHead("RecipeExp");

  static FactoryParser()
  {
    GrammarSource = File.ReadAllText(Resources.GetPathForResource("Grammar/factory.grammar"));

    parser = GenerateFactoryParser();
  }

  private static Parser<FactoryLexon> GenerateFactoryParser()
  {
    var grammar = GrammarParser.ParseGrammar([GrammarSource], FactoryLexonRules.lexonFromName);
    return new Parser<FactoryLexon>(grammar);
  }

  public static ParseResult<FactoryLexon> TryParse(string sourceCode)
  {
    return TryParse(FactoryLexer.LexFactory(sourceCode).Filter(x => x.isSemantic));
  }

  public static ParseResult<FactoryLexon> TryParse(Lexon<FactoryLexon>[] lexons)
  {
    return parser.TryParse("Program", lexons.Filter(x => x.isSemantic))!;
  }

  public static ASTNode<FactoryLexon>? Parse(string sourceCode)
  {
    var lexons = FactoryLexer.LexFactory(sourceCode);
    return parser.Parse("Program", lexons.Filter(x => x.isSemantic));
  }

  public static ASTNode<FactoryLexon>? Parse(Lexon<FactoryLexon>[] lexons) =>
    parser.Parse("Program", lexons.Filter(x => x.isSemantic));
}
