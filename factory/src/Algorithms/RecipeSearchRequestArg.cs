namespace Factory;

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
