namespace Factory;

/// <summary>
/// Represents a request to solve a recipe.
/// </summary>
public class RecipeSearchRequest
{
  /// <summary>
  /// The recipe to solve.
  /// </summary>
  public readonly LineValue recipe;

  /// <summary>
  /// The amount to solve for.
  /// </summary>
  public readonly decimal quantity;

  /// <summary>
  /// Create a new recipe search request
  /// </summary>
  /// <param name="recipe"></param>
  /// <param name="quantity"></param>
  public RecipeSearchRequest(LineValue recipe, decimal quantity)
  {
    this.recipe = recipe;
    this.quantity = quantity;
  }
}
