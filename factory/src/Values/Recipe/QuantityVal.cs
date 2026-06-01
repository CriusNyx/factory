using Factory;

public class QuantityVal(NumVal quantity, string item) : FactVal
{
  public NumVal Quantity => quantity;
  public string Item => item;

  public override string ToString()
  {
    return $"{Quantity} {Item}";
  }
}
