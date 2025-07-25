using SharpParse.Functional;
using SharpParse.Util;

namespace Factory;

public class AssignExpNode : ProgramExp
{
  public ExpChainNode left;
  public ValueNode right;

  public AssignExpNode() { }

  public AssignExpNode(SourceCodeInfo sourceInfo, ExpChainNode left, ValueNode right)
    : base(sourceInfo)
  {
    this.left = left;
    this.right = right;
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    var evaluationType = right.GetFactoryType(context);
    var assignType = left.ComputeRef(context);
    left.SetAssignType(evaluationType);

    if (
      evaluationType is FactoryPrimitiveType primType
      && primType.type == FactoryPrimitiveTypeType.Void
    )
    {
      var pos = right.Range;
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
    return [left, right];
  }

  public override (string?, string?) PrintSelf()
  {
    return ("let", null);
  }

  public override bool Equivalent(object other)
  {
    return other is AssignExpNode assign
      && left.Equivalent(assign.left)
      && right.Equivalent(assign.right);
  }

  public static AssignExpNode Create(SourceCodeInfo sourceInfo, ExpChainNode lhs, ValueNode rhs)
  {
    var output = new AssignExpNode();
    output.SetSourceInfo(sourceInfo);
    output.left = lhs;
    output.right = rhs;
    return output;
  }
}
