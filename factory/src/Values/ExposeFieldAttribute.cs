namespace Factory;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
public class ExposeMemberAttribute(string name) : Attribute
{
  public readonly string name = name;
}
