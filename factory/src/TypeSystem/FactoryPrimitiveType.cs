namespace Factory;

public class FactoryPrimitiveType(FactoryPrimitiveTypeType type) : FactoryType
{
  public readonly FactoryPrimitiveTypeType type = type;

  public bool CanAcceptValue(FactoryType other)
  {
    switch (type)
    {
      case FactoryPrimitiveTypeType.Void:
        return false;
      case FactoryPrimitiveTypeType.String:
        return CanCoerceToString(other);
      case FactoryPrimitiveTypeType.Number:
        return other is FactoryPrimitiveType primType
          && primType.type == FactoryPrimitiveTypeType.Number;
      default:
        throw new NotImplementedException();
    }
  }

  private bool CanCoerceToString(FactoryType otherType)
  {
    if (otherType is FactoryPrimitiveType primType)
    {
      switch (primType.type)
      {
        case FactoryPrimitiveTypeType.Number:
        case FactoryPrimitiveTypeType.String:
          return true;
      }
    }
    return false;
  }

  public override string ToString()
  {
    return type.ToString();
  }

  public string ToShortString()
  {
    return ToString();
  }
}
