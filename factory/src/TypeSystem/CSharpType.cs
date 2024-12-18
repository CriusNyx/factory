namespace Factory;

public class CSharpType(Type type) : FactoryType
{
  public readonly Type type = type;

  public bool CanAcceptValue(FactoryType other)
  {
    throw new NotImplementedException();
  }

  public override string ToString()
  {
    return type.ToString();
  }
}
