using Factory;
using Factory.Util;

public class RecipeType(RecipeNode recipeNode) : FactoryType
{
  public bool CanAcceptValue(FactoryType other)
  {
    // Not assignable
    return false;
  }

  public string ToShortString()
  {
    return $"{(recipeNode.Alt ? "alt " : "")}{PrintQuantitySet(recipeNode.Input)} => {PrintQuantitySet(recipeNode.Output)}";
  }

  private static string PrintQuantitySet(QuantityNode[] quantities)
  {
    if (quantities.Length == 0)
    {
      return "_";
    }
    else
    {
      return string.Join(" + ", quantities.Map(PrintQuantity));
    }
  }

  private static string PrintQuantity(QuantityNode quantity)
  {
    return $"{quantity.Quantity.Source} {quantity.Item.Source}";
  }
}
