using GenParse.Functional;

public static class SearchExtensions
{
  public static RecipeSearchResult Balance(this RecipeSearchResult result, bool hasInitialQuantity)
  {
    var limitArguments = result.request.recipe.arguments
      .Filter(x => x.IsLimitVal())
      .array.ToTypedArray<TypedFactVal>();

    if (limitArguments.Length == 0)
    {
      return result;
    }

    var targetLimitArg = limitArguments.Map((x) => ComputeLimitRatio(result, x)).Min();

    if (hasInitialQuantity && targetLimitArg > 1)
    {
      return result;
    }

    return new RecipeSearchResult(result.request, result.root.Multiply(targetLimitArg));
  }

  private static decimal ComputeLimitRatio(this RecipeSearchResult result, TypedFactVal limitVal)
  {
    var (quantity, symbol) = limitVal.ToLimitVal();
    var limitQuantity = result.recipeBalance.GetValueForIdentifier(symbol);
    if (limitQuantity == 0)
    {
      return 1;
    }
    return quantity / limitQuantity;
  }

  public static RecipeSearchNode Multiply(this RecipeSearchNode node, decimal value)
  {
    return new RecipeSearchNode(
      node.quantity * value,
      node.nodeName,
      node.recipe,
      node.item,
      node.children.Map(x => x.Multiply(value))
    );
  }
}
