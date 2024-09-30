using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;

public class RecipeNode : ProgramExpNode
{
  public RecipeExpNode[] children;

  public RecipeNode(
    Transformer<FactoryLexon, LanguageNode> transformer,
    ASTNode<FactoryLexon> astNode
  ) : base(transformer, astNode)
  {
    if (astNode.TryMatch(("recipeKeyword", "symbol", "RecipeExp*"), out var result))
    {
      var (_, _, recipeExpressions) = result;
      children = recipeExpressions.children.Map(x => RecipeExpNode.Transform(transformer, x));
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  public override string ToString()
  {
    return string.Join("\n", children.Map(x => x.ToString()));
  }
}
