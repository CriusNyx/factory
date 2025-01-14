namespace Factory;

public class RecipeSearchRequestTallyArg : RecipeSearchRequestArg
{
  public readonly string itemIdentifier;

  public RecipeSearchRequestTallyArg(string itemIdentifier)
  {
    this.itemIdentifier = itemIdentifier;
  }
}
