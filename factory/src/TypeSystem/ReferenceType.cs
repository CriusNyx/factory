namespace Factory;

public class ReferenceType(string symbol) : FactoryType
{
  public readonly string symbol = symbol;

  public bool CanAcceptValue(FactoryType other)
  {
    throw new NotImplementedException();
  }

  public string ToShortString()
  {
    return $"typeof {symbol}";
  }
}
