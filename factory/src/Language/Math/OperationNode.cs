using Factory;
using SharpParse.Parsing;
using SharpParse.Util;

[ASTClass("FactorOperation", "TermOperation")]
public class OperationNode(ASTNode astNode) : LanguageNode
{
  public readonly string operation = astNode.children.First().SourceCode();

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
