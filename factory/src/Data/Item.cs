using GenParse.Functional;

[Serializable]
public class Item
{
  public string className;
  public string name;
  public string displayName;

  public string identifier => displayName.Replace(" ", "");
  public string form;

  public decimal ComputeUIConversionRate()
  {
    return ItemForm.conversionRates.SafeGet(form, 1);
  }
}

public static class ItemForm
{
  public const string LIQUID = "RF_LIQUID";
  public const string GAS = "RF_GAS";
  public static Dictionary<string, decimal> conversionRates = new Dictionary<string, decimal>()
  {
    { LIQUID, 1000 },
    { GAS, 1000 },
  };
}
