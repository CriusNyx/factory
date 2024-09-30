using Factory.Parsing;
using GenParse.Parsing;

public abstract class RecipeExpNode : LanguageNode
{
  public RecipeExpNode(
    Transformer<FactoryLexon, LanguageNode> transformer,
    ASTNode<FactoryLexon> astNode
  ) : base(transformer, astNode) { }

  public static RecipeExpNode Transform(
    Transformer<FactoryLexon, LanguageNode> transformer,
    ASTNode<FactoryLexon> astNode
  )
  {
    if (astNode.TryMatch("OutExp", out var outExp))
    {
      return new OutExpNode(transformer, outExp);
    }
    else if (astNode.TryMatch("InExp", out var inExp))
    {
      return new InExpNode(transformer, inExp);
    }
    else if (astNode.TryMatch("AltExp", out var altExp))
    {
      return new AltExpNode(transformer, altExp);
    }
    throw new NotImplementedException();
  }
}
