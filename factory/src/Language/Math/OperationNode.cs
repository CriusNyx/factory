using Factory;
using SharpParse.Util;

public class OperationNode(string operation) : LanguageNode
{
  // TODO: Make this throw not implemented until refactor is done.
  public string Operation => operation;

  public override FactoryType CalculateType(TypeContext context)
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
}
