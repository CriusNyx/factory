using GenParse.Functional;

[Serializable]
public class Quantity
{
  public string itemClass;
  public decimal amount;

  public Item? item => Docs.itemsByClass.Safe(itemClass);

  public string identifier => item?.identifier ?? itemClass;

  public override string ToString()
  {
    return $"{amount / item?.ComputeUIConversionRate() ?? 1} {identifier}";
  }

  public string ToString(decimal productionRate)
  {
    return $"{amount / item?.ComputeUIConversionRate() * productionRate ?? 1} {identifier}";
  }

  public static string GetDisplayName(Quantity? quantity)
  {
    return quantity == null ? "nil" : quantity.ToString();
  }
}
