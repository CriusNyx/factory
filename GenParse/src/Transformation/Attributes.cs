[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ASTAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class ASTClassAttribute(string nodeName) : Attribute
{
  public readonly string nodeName = nodeName;
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ASTFieldAttribute : Attribute
{
  public readonly string grammarElementName;

  public ASTFieldAttribute(string grammarElementName)
  {
    this.grammarElementName = grammarElementName;
  }
}

public interface ASTTransformer
{
  object Transform();
}
