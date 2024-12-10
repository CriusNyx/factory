using GenParse.Functional;

[Serializable]
public class Recipe : FactVal, IFunc
{
  private static string[] nonAlternateRecipes = new string[]
  {
    "Alternate: Compacted Coal",
    "Alternate: Turbofuel",
    "Alternate: Heavy Oil Residue",
  };

  public string className;
  public string displayName;
  public string slug;
  public bool isAlternative =>
    displayName.StartsWith("Alternate: ") && !nonAlternateRecipes.Contains(displayName);
  public bool machineCraftable;
  public Quantity[] ingredients;
  public Quantity[] product;
  public string[] producedIn;
  public decimal manufactoringDuration;

  public string primaryProductClass => product?.FirstOrDefault()?.itemClass ?? "";
  public Item primaryProduct => Docs.itemsByClass!.Safe(product.FirstOrDefault()?.itemClass)!;

  public string identifier =>
    className == "Recipe_CartridgeChaos_Packaged_C"
      ? "PackagedTurboRifleAmmo"
      : displayName
        .Replace("Alternate: ", "")
        .Replace(" ", "")
        .Replace("-", "")
        // TODO: Make special case for ore conversion.
        .Replace("(", "")
        .Replace(")", "");

  public bool isMachineRecipe => producedIn.Any(y => Docs.ProductionMachines.Contains(y));

  public override string ToString()
  {
    return $"{identifier}:".PadRight(50) + $"{GetQuantity(ingredients)} -> {GetQuantity(product)}";
  }

  private string GetQuantity(Quantity[]? quantity)
  {
    if (quantity == null)
    {
      return "nil";
    }
    else if (quantity.Length == 0)
    {
      return "nil";
    }
    else
      return string.Join(" + ", quantity.Map(x => x.ToString(60 / manufactoringDuration)));
  }

  public FactVal Invoke(ArrayVal arguments)
  {
    return RecipeInvocation.InvokeRecipe(this, arguments);
  }
}
