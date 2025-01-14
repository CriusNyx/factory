using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("AssignExp")]
public class AssignExpNode : ProgramExp
{
  public override ASTNode<FactoryLexon> astNode => _astNode;

  public ASTNode<FactoryLexon> _astNode { get; set; }

  [ASTField("ExpChain")]
  public ExpChainNode left;

  [ASTField("ValueExp")]
  public ValueNode right;

  public override FactoryType CalculateType(TypeContext context)
  {
    var evaluationType = right.GetFactoryType(context);
    var assignType = left.ComputeRef(context);
    if (
      evaluationType is FactoryPrimitiveType primType
      && primType.type == FactoryPrimitiveTypeType.Void
    )
    {
      var pos = right.astNode.CalculatePosition();
      context.AddError(pos.start, pos.length, $"Cannot resolve value from expression");
    }
    if (assignType is ReferenceType refType)
    {
      context.SetType(refType.symbol, evaluationType.ResolveType(context));
    }
    else { }
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Void);
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var reference = left.GetReference(context);
    var result = right.Evaluate(ref context);
    reference.Set(result);
    return result.With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { left, right };
  }

  public override (string?, string?) PrintSelf()
  {
    return ("let", null);
  }
}
