using GenParse.Functional;

namespace Factory;

public static class SearchExtensions
{
  public static RecipeSolution Balance(this RecipeSolution result, bool hasInitialQuantity)
  {
    var limitArguments = result.request.recipe.arguments.limitVals;

    if (limitArguments.Length == 0)
    {
      return result;
    }

    var targetLimitArg = limitArguments.Map((x) => ComputeLimitRatio(result, x)).Min();

    if (hasInitialQuantity && targetLimitArg > 1)
    {
      return result;
    }

    return new RecipeSolution(result.request, result.root.Multiply(targetLimitArg));
  }

  private static decimal ComputeLimitRatio(this RecipeSolution result, LimitVal limitVal)
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
