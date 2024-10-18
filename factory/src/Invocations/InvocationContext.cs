public class InvocationContext(RecipeValue recipeValue, decimal quantity = 1, bool hasQuantityValue = false)
{
  public readonly RecipeValue recipeValue = recipeValue;
  public readonly bool hasQuantityValue = hasQuantityValue;
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

  public FactVal Invoke(){
    var searchRequest = new RecipeSearchRequest(recipeValue, quantity);
    return RecipeSearch.Search(searchRequest).Balance(hasQuantityValue);
  }
}
