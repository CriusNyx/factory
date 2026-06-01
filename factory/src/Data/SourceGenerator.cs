using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Nodes;
using Factory;
using Factory.Util;
using Newtonsoft.Json;

public static class SourceGenerator
{
  public static string GenerateSatisfactorySource()
  {
    StringBuilder builder = new StringBuilder();

    foreach (var resource in RecipeSearch.resourceIdentifiers)
    {
      builder.AppendLine($"resource {resource}");
    }

    builder.AppendLine();

    var recipes = Docs.recipesByIdentifier.Values.SelectMany(x => x);

    foreach (var recipe in recipes)
    {
      if (recipe.isAlternative)
      {
        continue;
      }
      builder.AppendLine(
        $"recipe {recipe.identifier} = {PrintRecipeQuantities(recipe.ingredients)} => {PrintRecipeQuantities(recipe.product)}"
      );
    }

    builder.AppendLine();

    foreach (var recipe in recipes)
    {
      if (!recipe.isAlternative)
      {
        continue;
      }
      builder.AppendLine(
        $"recipe alt {recipe.identifier} = {PrintRecipeQuantities(recipe.ingredients)} => {PrintRecipeQuantities(recipe.product)}"
      );
    }

    return builder.ToString();
  }

  private static string PrintRecipeQuantities(Quantity[]? quantities)
  {
    if (quantities == null || quantities.Length == 0)
    {
      return "_";
    }
    return String.Join(" + ", quantities.Map(x => $"{x.Amount} {x.identifier}"));
  }
}
