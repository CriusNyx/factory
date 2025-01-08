namespace Factory;

public class CSharpType(Type type) : FactoryType
{
  public readonly Type type = type;

  public bool CanAcceptValue(FactoryType other)
  {
    if (other is CSharpType csType)
    {
      return csType.type.IsAssignableTo(type);
    }
    else if (type == typeof(FactVal))
    {
      return true;
    }
    return false;
  }

  public override string ToString()
  {
    return $"CSharpType({type})";
  }
}
