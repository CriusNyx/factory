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

  public RecipeInvocation Amend(FactVal factVal)
  {
    if (RecipeValue.FactValModifiesRecipeVal(factVal))
    {
      return Clone(recipeValue: recipeValue.Amend(factVal));
    }
    else if (factVal is NumVal number)
    {
      return Clone(quantity: number.value);
    }
    return this;
  }

  public RecipeInvocation Clone(RecipeValue? recipeValue = null, decimal? quantity = null)
  {
    return new RecipeInvocation(recipeValue ?? this.recipeValue, quantity ?? this.quantity);
  }

  public RecipeSearchResult Invoke()
  {
    var searchRequest = new RecipeSearchRequest(recipeValue, quantity);
    return RecipeSearch.Search(searchRequest).Balance(hasQuantityValue);
  }

  public static RecipeSearchResult InvokeRecipe(FactVal recipe, FactVal[] invocationParams)
  {
    var recVal = GetRecipeForInvocation(recipe)
      .AmendInvocation(invocationParams.Filter(x => !(x is NumVal)));
    decimal quantity = 1;
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
      return new RecipeValue(
        recipe.identifier,
        new ArrayVal(
          new TypedFactVal(ValType.output, new SymbolVal(recipe.primaryProduct.identifier))
        )
      );
    }
    throw new InvalidOperationException($"Could not resolve invocation on object {o}");
  }
}
