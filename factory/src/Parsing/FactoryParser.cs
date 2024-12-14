using GenParse.Functional;
using GenParse.Grammar;
using GenParse.Lexing;
using GenParse.Parsing;

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

  public static Parser<FactoryLexon> GenerateFactoryParser()
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

  public static ASTNode<FactoryLexon>? Parse(string sourceCode, bool recover = false)
  {
    var lexons = FactoryLexer.LexFactory(sourceCode);
    return parser.Parse("Program", lexons.Filter(x => x.isSemantic), recover);
  }

  public static ASTNode<FactoryLexon>? Parse(
    Lexon<FactoryLexon>[] lexons,
    bool forgiving = false
  ) => parser.Parse("Program", lexons.Filter(x => x.isSemantic), forgiving);
}
