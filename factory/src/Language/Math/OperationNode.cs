using Factory;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("FactorOperation", "TermOperation")]
public class OperationNode(ASTNode<FactoryLexon> astNode) : LanguageNode
{
  public readonly string operation = astNode.children.First().SourceCode();

  public FactoryType CalculateType(TypeContext context)
  {
    return FactoryType.VoidType;
  }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [];
  }
}
