using Factory.Util;

namespace Factory;

public class ProgramNode : LanguageNode
{
  public ProgramExp[] expressions;

  public ProgramNode() { }

  public ProgramNode(SourceCodeInfo sourceInfo, ProgramExp[] expressions)
    : base(sourceInfo)
  {
    this.expressions = expressions;
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren() =>
    expressions.ToTypedArray<Formatting.ITree<LanguageNode>>();

  public void Evaluate(ExecutionContext executionContext)
  {
    foreach (var expression in expressions)
    {
      (_, executionContext) = expression.Evaluate(executionContext);
    }
  }

  public string ToTree()
  {
    return Formatting.PrintTree(this, x => x.ToString()!);
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    foreach (var expression in expressions)
    {
      expression.GetFactoryType(context);
    }
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Void);
  }
}

public abstract class ProgramExp : ValueNode
{
  public ProgramExp() { }

  public ProgramExp(SourceCodeInfo sourceInfo)
    : base(sourceInfo) { }
}
