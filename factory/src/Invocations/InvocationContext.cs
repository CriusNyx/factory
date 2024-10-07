public class InvocationContext(RecipeValue recipeValue, decimal quantity = 0)
{
  public readonly RecipeValue recipeValue = recipeValue;
  public readonly decimal quantity = quantity;

  public InvocationContext Amend(FactVal factVal)
  {
    if(RecipeValue.FactValModifiesRecipeVal(factVal)){
      return Clone(recipeValue: recipeValue.Amend(factVal));
    }
    else if(factVal is NumVal number){
      return Clone(quantity: number.value);
    }
    return this;
  }

  public InvocationContext Clone(RecipeValue? recipeValue = null, decimal? quantity = null){
    return new InvocationContext(recipeValue ?? this.recipeValue, quantity ?? this.quantity);
  }

  public RecipeSearchResult Invoke(){
    var searchRequest = new RecipeSearchRequest(recipeValue, quantity);
    return RecipeSearch.Search(searchRequest);
  }
}
