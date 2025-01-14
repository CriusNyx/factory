namespace Factory;

public class RecipeSearchRequestQuantityArg : RecipeSearchRequestArg
{
  public readonly decimal quantity;

  public RecipeSearchRequestQuantityArg(decimal quantity)
  {
    this.quantity = quantity;
  }
}
