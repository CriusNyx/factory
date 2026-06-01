namespace Factory;

public interface HasRecipe
{
  public RecipeVal Recipe { get; }
}

public interface HasResource
{
  public ResourceVal Resource { get; }
}
