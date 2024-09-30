public class RecipeSearchRequest
{
  public readonly RecipeValue recipe;
  public readonly RecipeSearchRequestArg[] args;

  public RecipeSearchRequest(RecipeValue recipe, RecipeSearchRequestArg[] args)
  {
    this.recipe = recipe;
    this.args = args;
  }
}

public class RecipeSearchRequestArg
{
  public static RecipeSearchRequestArg CreateArgFromRawValue(object value)
  {
    if (value is RecipeSearchRequestArg arg)
    {
      return arg;
    }
    else if (value is decimal dec)
    {
      return new RecipeSearchRequestQuantityArg(dec);
    }
    else
    {
      throw new NotImplementedException();
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
