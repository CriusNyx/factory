using SharpParse.Functional;
using SharpParse.Grammar;
using SharpParse.Lexing;
using SharpParse.Parsing;

namespace Factory;

public static class FactoryParser
{
  public static string GrammarSource { get; private set; }

  public static readonly Parser parser;

  public static string[] ProgramLexonHeads => parser.ComputeHead("Program");
  public static string[] RecipeExpLexonHeads => parser.ComputeHead("RecipeExp");

  static FactoryParser()
  {
    GrammarSource = File.ReadAllText(Resources.GetPathForResource("Grammar/factory.grammar"));

    parser = GenerateFactoryParser();
  }

  private static Parser GenerateFactoryParser()
  {
    var grammar = GrammarParser.ParseGrammar([GrammarSource], (x) => x);
    return new Parser(grammar);
  }

  public static ParseResult TryParse(string sourceCode)
  {
    return TryParse(FactoryLexer.LexFactory(sourceCode).Filter(x => x.isSemantic));
  }

  public static ParseResult TryParse(Lexon[] lexons)
  {
    return parser.TryParse("Program", lexons.Filter(x => x.isSemantic))!;
  }

  public static ASTNode? Parse(string sourceCode)
  {
    var lexons = FactoryLexer.LexFactory(sourceCode);
    return parser.Parse("Program", lexons.Filter(x => x.isSemantic));
  }

  public static ASTNode? Parse(Lexon[] lexons) =>
    parser.Parse("Program", lexons.Filter(x => x.isSemantic));
}
