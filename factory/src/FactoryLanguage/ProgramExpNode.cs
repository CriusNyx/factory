using Factory.Parsing;
using GenParse.Parsing;

public abstract class ProgramExpNode : LanguageNode
{
  protected ProgramExpNode(
    Transformer<FactoryLexon, LanguageNode> transformer,
    ASTNode<FactoryLexon> astNode
  ) : base(transformer, astNode) { }

  public static ProgramExpNode Transform(
    Transformer<FactoryLexon, LanguageNode> transformer,
    ASTNode<FactoryLexon> astNode
  )
  {
    if (astNode.TryMatch("Recipe", out var recipeNode))
    {
      return new RecipeNode(transformer, recipeNode);
    }
    throw new NotImplementedException($"Unknown ast node type {astNode}");
  }
}
