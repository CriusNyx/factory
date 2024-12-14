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

  public FactVal Invoke()
  {
    var searchRequest = new RecipeSearchRequest(recipeValue, quantity);
    return RecipeSearch.Search(searchRequest).Balance(hasQuantityValue);
  }

  public static FactVal InvokeRecipe(FactVal recipe, ArrayVal invocationParams)
  {
    var invocation = invocationParams.array.Reduce(
      new RecipeInvocation(GetRecipeForInvocation(recipe)),
      (factVal, context) => context.Amend(factVal)
    );
    return invocation.Invoke();
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
