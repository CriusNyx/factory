using GenParse.Functional;

public static class RecipeSearch
{
  static string[] resourceIdentifiers = new string[]
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
    "SAM",
    "Water"
  };

  public static RecipeSearchResult Search(RecipeSearchRequest request)
  {
    var rootItemIdentifier = request.recipe.arguments.ExtractSymbolsOfType(ValType.output).First();
    var rootItem = Docs.itemsByIdentifier.Safe(rootItemIdentifier)!;

    var root = ResolveRecipe(request.recipe, request.quantity * rootItem.ComputeUIConversionRate());

    return new RecipeSearchResult(request, root!);
  }

  public static RecipeSearchNode? ResolveRecipe(RecipeValue recipeValue, decimal amount)
  {
    var output = recipeValue.arguments.ExtractSymbolsOfType(ValType.output).First();
    return MakeResult(ResolveRecipe(output, recipeValue, amount), recipeValue);
  }

  public static RecipeSearchNode? MakeResult(
    (Recipe? recipe, string? identifier, decimal quantity) input,
    RecipeValue context
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
      recipe.ingredients
        .Map(
          x => MakeResult(ResolveRecipe(x.identifier, context, recipeQuantity * x.Amount), context)
        )
        .Push(MakeBiproducts(recipe, recipeQuantity))
        .Filter(x => x != null)!,
      recipe.primaryProduct
    );
  }

  public static RecipeSearchNode[] MakeBiproducts(Recipe? recipe, decimal quantity)
  {
    if (recipe == null)
    {
      return new RecipeSearchNode[] { };
    }
    return recipe.product
      .Spread(1)
      .Map(x => new RecipeSearchNode(x.identifier, -quantity * x.Amount, x.item!));
  }

  public static (Recipe? recipe, string? name, decimal quantity) ResolveRecipe(
    string itemIdentifier,
    RecipeValue context,
    decimal amount
  )
  {
    var inputs = context.arguments.ExtractSymbolsOfType(ValType.input);
    var alts = context.arguments.ExtractSymbolsOfType(ValType.alt);

    if (resourceIdentifiers.Contains(itemIdentifier) || inputs.Contains(itemIdentifier))
    {
      return (null, itemIdentifier, amount);
    }
    var itemClassName = Docs.itemsByIdentifier[itemIdentifier].className;
    var defaultRecipe = Docs.recipesByProductClass[itemClassName].FirstOrDefault(
      x => !x.isAlternative && x.isMachineRecipe
    );
    var altRecipe = Docs.recipesByProductClass[itemClassName].FirstOrDefault(
      x => alts.Contains(x.identifier) && x.isMachineRecipe
    );
    return (altRecipe ?? defaultRecipe!, null, amount);
  }
}
