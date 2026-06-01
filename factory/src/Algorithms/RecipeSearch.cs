using Factory.Util;

namespace Factory;

public static class RecipeSearch
{
  public static string[] resourceIdentifiers = new string[]
  {
    "Limestone",
    "IronOre",
    "CopperOre",
    "CateriumOre",
    "Coal",
    "Sulfur",
    "Bauxite",
    "RawQuartz",
    "Uranium",
    "CrudeOil",
    "Nitrogen",
    "SAM",
    "Water",
    "Leaves",
    "Wood",
    "BaconAgaric",
    "Berry",
    "Nut",
    "ExcitedPhotonicMatter",
    "MercerSphere",
    "Mycelia",
    "Somersloop",
    "Gift",
  };

  public static RecipeSolution Search(RecipeSearchRequest request)
  {
    var recOut = request.recipe.arguments.outVal;
    var rootItem = Docs.itemsByIdentifier.Safe(recOut.identifier)!;

    var root = ResolveRecipe(request.recipe, request.quantity * rootItem.ComputeUIConversionRate());

    return new RecipeSolution(request, root!);
  }

  public static RecipeSearchNode? ResolveRecipe(LineValue recipeValue, decimal amount)
  {
    var recOut = recipeValue.arguments.outVal;
    return MakeResult(ResolveRecipe(recOut.identifier, recipeValue, amount), recipeValue);
  }

  public static RecipeSearchNode? MakeResult(
    (SatisfactoryRecipe? recipe, string? identifier, decimal quantity) input,
    LineValue context
  )
  {
    var (recipe, identifier, amount) = input;
    if (recipe == null)
    {
      return new RecipeSearchNode(identifier!, amount, Docs.itemsByIdentifier!.Safe(identifier)!);
    }
    decimal recipeQuantity = amount / recipe.product.First().Amount;
    return new RecipeSearchNode(
      recipe,
      recipeQuantity,
      recipe
        .ingredients.Map(x =>
          MakeResult(ResolveRecipe(x.identifier, context, recipeQuantity * x.Amount), context)
        )
        .Push(MakeBiproducts(recipe, recipeQuantity))
        .FilterDefined(),
      recipe.primaryProduct
    );
  }

  public static RecipeSearchNode[] MakeBiproducts(SatisfactoryRecipe? recipe, decimal quantity)
  {
    if (recipe == null)
    {
      return new RecipeSearchNode[] { };
    }
    return recipe
      .product.Spread(1)
      .Map(x => new RecipeSearchNode(x.identifier, -quantity * x.Amount, x.item!));
  }

  public static (SatisfactoryRecipe? recipe, string? name, decimal quantity) ResolveRecipe(
    string itemIdentifier,
    LineValue context,
    decimal amount
  )
  {
    var inputs = context.arguments.inVals.Map(x => x.identifier);
    var alts = context.arguments.altVals.Map(x => x.identifier);

    if (resourceIdentifiers.Contains(itemIdentifier) || inputs.Contains(itemIdentifier))
    {
      return (null, itemIdentifier, amount);
    }
    var itemClassName = Docs.itemsByIdentifier[itemIdentifier].className;
    var defaultRecipe = Docs.recipesByProductClass[itemClassName]
      .FirstOrDefault(x => !x.isAlternative && x.isMachineRecipe);
    var altRecipe = Docs.recipesByProductClass[itemClassName]
      .FirstOrDefault(x => alts.Contains(x.identifier) && x.isMachineRecipe);
    return (altRecipe ?? defaultRecipe!, null, amount);
  }
}
