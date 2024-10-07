public class RecipeSearchRequest
{
  public readonly RecipeValue recipe;
  public readonly decimal quantity;

  public RecipeSearchRequest(RecipeValue recipe, decimal quantity)
  {
    this.recipe = recipe;
    this.quantity = quantity;
  }
}

public class RecipeSearchRequestArg : FactVal
{
  public static RecipeSearchRequestArg? CreateArgFromRawValue(object value)
  {
    if (value is RecipeSearchRequestArg arg)
    {
      return arg;
    }
    else if (value is NumVal num)
    {
      return new RecipeSearchRequestQuantityArg(num.value);
    }
    else
    {
      return null;
    }
  }
}

public class RecipeSearchRequestQuantityArg : RecipeSearchRequestArg
{
  public readonly decimal quantity;

  public RecipeSearchRequestQuantityArg(decimal quantity)
  {
    this.quantity = quantity;
  }
}

public class RecipeSearchRequestTallyArg : RecipeSearchRequestArg
{
  public readonly string itemIdentifier;

  public RecipeSearchRequestTallyArg(string itemIdentifier)
  {
    this.itemIdentifier = itemIdentifier;
  }
}
