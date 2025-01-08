using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("SpreadExp")]
public class SpreadExpNode(ASTNode<FactoryLexon> astNode) : RecipeExpNode, LanguageNode
{
  private ASTNode<FactoryLexon> _astNode = astNode;
  public override ASTNode<FactoryLexon> astNode => _astNode;

  [ASTField("symbol")]
  public SymbolNode symbol;

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var target = context.Resolve(symbol.Evaluate());
    var spreadMethod = target?.GetType().GetFactorySpreadMethod();
    return (spreadMethod?.Invoke(target, new object[] { }) as FactVal).NotNull().With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { symbol };
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Complex);
  }
}
