namespace Factory;

public class RecipeAndResourceVal(ResourceVal resource, RecipeVal recipe)
  : FactVal,
    HasRecipe,
    HasResource
{
  public ResourceVal Resource => resource;

  public RecipeVal Recipe => recipe;
}
