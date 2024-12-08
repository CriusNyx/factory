[AttributeUsage(AttributeTargets.Class)]
public class ASTClass(string nodeName) : Attribute
{
  public readonly string nodeName = nodeName;
}
