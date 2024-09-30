using Factory.Parsing;
using GenParse.Parsing;

public class FactoryTransformer : Transformer<FactoryLexon, LanguageNode>
{
  private static (string, Func<
    Transformer<FactoryLexon, LanguageNode>,
    ASTNode<FactoryLexon>,
    LanguageNode
  >)[] GenerateRules()
  {
    return
    [
      ("Program", (transformer, astNode) => new ProgramNode(transformer, astNode))
    ];
  }

  public FactoryTransformer() : base(GenerateRules()) { }
}
