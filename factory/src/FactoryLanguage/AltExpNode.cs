using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;

public class AltExpNode : RecipeExpNode
{
  public string[] altValues;

  public AltExpNode(
    Transformer<FactoryLexon, LanguageNode> transformer,
    ASTNode<FactoryLexon> astNode
  ) : base(transformer, astNode)
  {
    if (astNode.TryMatch(("altKeyword", "symbol*"), out var result))
    {
      var (_, symbols) = result;
      altValues = symbols.children.Map(x => x.lexons.First().sourceCode);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  public override string ToString()
  {
    return $"alt {string.Join(" ", altValues)}";
  }
}
