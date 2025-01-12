using GenParse.Functional;

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

  public string ToShortString()
  {
    return type.ToString();
  }

  public override string ToString()
  {
    return $"CSharpType({type})";
  }
}
