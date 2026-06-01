using Factory.Util;

namespace Factory;

public class RecipeInvocation(
  LineValue recipeValue,
  decimal quantity = 1,
  bool hasQuantityValue = false
)
{
  public readonly LineValue recipeValue = recipeValue;
  public readonly bool hasQuantityValue = hasQuantityValue;
  public readonly decimal quantity = quantity;

  public RecipeInvocation Clone(LineValue? recipeValue = null, decimal? quantity = null)
  {
    return new RecipeInvocation(recipeValue ?? this.recipeValue, quantity ?? this.quantity);
  }

  public RecipeSolution Invoke()
  {
    var searchRequest = new RecipeSearchRequest(recipeValue, quantity);
    return RecipeSearch.Search(searchRequest).Balance(hasQuantityValue);
  }

  public static RecipeSolution InvokeRecipe(
    FactVal recipe,
    decimal quantity,
    FactVal[] invocationParams
  )
  {
    var recVal = GetRecipeForInvocation(recipe)
      .AmendInvocation(invocationParams.FilterByType<FactVal, LineArgSet>());
    bool hasQuantity = false;
    foreach (var param in invocationParams)
    {
      if (param is NumVal numVal)
      {
        quantity = numVal.value;
        hasQuantity = true;
      }
    }
    return new RecipeInvocation(recVal, quantity, hasQuantity).Invoke();
  }

  private static LineValue GetRecipeForInvocation(object o)
  {
    if (o is LineValue recipeValue)
    {
      return recipeValue;
    }
    else if (o is SatisfactoryRecipe recipe)
    {
      return new LineValue(recipe.identifier, new LineArgSet([new OutVal(recipe.identifier)]));
    }
    throw new InvalidOperationException($"Could not resolve invocation on object {o}");
  }
}
