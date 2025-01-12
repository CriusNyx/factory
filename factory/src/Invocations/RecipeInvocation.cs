using GenParse.Functional;

namespace Factory;

public class RecipeInvocation(
  RecipeValue recipeValue,
  decimal quantity = 1,
  bool hasQuantityValue = false
)
{
  public readonly RecipeValue recipeValue = recipeValue;
  public readonly bool hasQuantityValue = hasQuantityValue;
  public readonly decimal quantity = quantity;

  public RecipeInvocation Clone(RecipeValue? recipeValue = null, decimal? quantity = null)
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
      .AmendInvocation(invocationParams.FilterByType<FactVal, RecipeArgSet>());
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

  private static RecipeValue GetRecipeForInvocation(object o)
  {
    if (o is RecipeValue recipeValue)
    {
      return recipeValue;
    }
    else if (o is Recipe recipe)
    {
      return new RecipeValue(recipe.identifier, new RecipeArgSet([new OutVal(recipe.identifier)]));
    }
    throw new InvalidOperationException($"Could not resolve invocation on object {o}");
  }
}
