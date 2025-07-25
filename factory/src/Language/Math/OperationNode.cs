using Factory;
using SharpParse.Util;

[ASTClass("FactorOperation", "TermOperation")]
public class OperationNode : LanguageNode
{
  // TODO: Make this throw not implemented until refactor is done.
  public string operation => astNode.children.First().SourceCode();

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
    return (operation, null);
  }
}
