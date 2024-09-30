using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;

public class InExpNode : RecipeExpNode
{
  public string[] inValues;

  public InExpNode(
    Transformer<FactoryLexon, LanguageNode> transformer,
    ASTNode<FactoryLexon> astNode
  ) : base(transformer, astNode)
  {
    if (astNode.TryMatch(("inKeyword", "symbol*"), out var result))
    {
      var (_, symbols) = result;
      inValues = symbols.children.Map(x => x.lexons.First().sourceCode);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  public override string ToString()
  {
    return $"in {string.Join(" ", inValues)}";
  }
}
