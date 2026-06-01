using Factory;
using Factory.Util;

public class OperationNode : LanguageNode
{
  public string Operation;

  public OperationNode(string operation)
  {
    Operation = operation;
  }

  public OperationNode(SourceCodeInfo sourceCodeInfo)
    : base(sourceCodeInfo)
  {
    Operation = sourceCodeInfo.Source;
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    return FactoryType.VoidType;
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [];
  }

  public override (string?, string?) PrintSelf()
  {
    return (Operation, null);
  }

  public override bool Equivalent(object other)
  {
    return other is OperationNode op && Operation == op.Operation;
  }
}
