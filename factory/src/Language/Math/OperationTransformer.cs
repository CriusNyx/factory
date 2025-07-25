using SharpParse.Parsing;

public class OperationTransformer(ASTNode astNode) : ASTTransformer
{
  public object Transform()
  {
    return new OperationNode(astNode.children.First().SourceCode());
  }
}
