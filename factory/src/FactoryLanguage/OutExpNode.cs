using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;

public class OutExpNode : RecipeExpNode
{
  public string[] outValues;

  public OutExpNode(
    Transformer<FactoryLexon, LanguageNode> transformer,
    ASTNode<FactoryLexon> astNode
  ) : base(transformer, astNode)
  {
    if (astNode.TryMatch(("outKeyword", "symbol*"), out var result))
    {
      var (_, symbols) = result;
      outValues = symbols.children.Map(x => x.lexons.First().sourceCode);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  public override string ToString()
  {
    return $"out {string.Join(" ", outValues)}";
  }
}
