using System.Reflection.Metadata.Ecma335;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("LimitExp")]
public class LimitExpNode(ASTNode<FactoryLexon> astNode) : RecipeExpNode
{
  private ASTNode<FactoryLexon> _astNode = astNode;
  public override ASTNode<FactoryLexon> astNode => _astNode;

  [ASTField("LimitValueExp*")]
  public LimitValueExpNode[] expressions;

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return expressions;
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return expressions.Map(x => x.Evaluate(ref context)).ToRecipeArgSet().With(context);
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    foreach (var exp in expressions)
    {
      exp.GetFactoryType(context);
    }
    return FactoryType.FromCSharpType(typeof(RecipeArgSet));
  }

  public override (string?, string?) PrintSelf()
  {
    return ("limit", null);
  }
}
