using GenParse.Functional;
using Newtonsoft.Json;

[Serializable]
public class Quantity
{
  public string itemClass;

  private decimal internalAmount;
  public decimal Amount => internalAmount / item?.ComputeUIConversionRate() ?? 1;

  public Item? item => Docs.itemsByClass.Safe(itemClass);

  public string identifier => item?.identifier ?? itemClass;

  public Quantity(decimal amount)
  {
    this.internalAmount = amount;
  }

  public override string ToString()
  {
    return $"{Amount} {identifier}";
  }

  public string ToString(decimal productionRate)
  {
    return $"{Amount * productionRate} {identifier}";
  }

  public static string GetDisplayName(Quantity? quantity)
  {
    return quantity == null ? "nil" : quantity.ToString();
  }
}
