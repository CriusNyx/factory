using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("LimitExp")]
public class LimitExpNode(ASTNode<FactoryLexon> astNode) : RecipeExpNode, LanguageNode
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
    return new ArrayVal(expressions.Map(x => x.Evaluate(ref context))).With(context);
  }
}