[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ExposeMemberAttribute(string name) : Attribute
{
  public readonly string name = name;
}
