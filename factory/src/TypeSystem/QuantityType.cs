using Factory;

public class QuantityType(QuantityNode node) : FactoryType
{
  public string Quantity => node.Quantity.Source;
  public string Item => node.Item.Source;

  public bool CanAcceptValue(FactoryType other)
  {
    // Not assignable.
    return false;
  }

  public string ToShortString()
  {
    return $"{Quantity} {Item}";
  }
}
