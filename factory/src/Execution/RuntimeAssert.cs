using SharpParse.Parsing;

namespace Factory;

public class RuntimeAssertException : Exception
{
  public RuntimeAssertException(string message)
    : base(message) { }
}

public static class RuntimeAssert
{
  public static void ASTNodeType(ASTNode<FactoryLexon> astNode, string type)
  {
    if (astNode.name != type)
    {
      throw new RuntimeAssertException($"ASTNode {astNode} does not have the expected type {type}");
    }
  }
}
