using GenParse.Functional;

public static class RecipeSearch
{
  static string[] terminalSymbols = new string[] { "IronOre", "CopperOre", "Coal", "CrudeOil" };

  public static RecipeSearchResult Search(RecipeSearchRequest request)
  {
    var quantityArg = request.args.FindByType<
      RecipeSearchRequestArg,
      RecipeSearchRequestQuantityArg
    >();

    if (quantityArg == null)
    {
      throw new InvalidOperationException(
        "You must provide a quantity argument to compute a recipe."
      );
    }

    var rootItemIdentifier = request.recipe.outputs.First();
    var rootItem = Docs.itemsByIdentifier.Safe(rootItemIdentifier)!;

    var root = ResolveRecipe(
      request.recipe,
      quantityArg.quantity * rootItem.ComputeUIConversionRate()
    );

    return new RecipeSearchResult(request, root!);
  }

  public static RecipeSearchNode? ResolveRecipe(RecipeValue recipeValue, decimal amount)
  {
    return MakeResult(ResolveRecipe(recipeValue.outputs.First(), recipeValue, amount), recipeValue);
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
    decimal recipeQuantity = amount / recipe.product.First().amount;
    return new RecipeSearchNode(
      recipe,
      recipeQuantity,
      recipe.ingredients
        .Map(
          x => MakeResult(ResolveRecipe(x.identifier, context, recipeQuantity * x.amount), context)
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
      .Map(x => new RecipeSearchNode(x.identifier, -quantity * x.amount, x.item!));
  }

  public static (Recipe? recipe, string? name, decimal quantity) ResolveRecipe(
    string itemIdentifier,
    RecipeValue context,
    decimal amount
  )
  {
    if (terminalSymbols.Contains(itemIdentifier) || context.inputs.Contains(itemIdentifier))
    {
      return (null, itemIdentifier, amount);
    }
    var itemClassName = Docs.itemsByIdentifier[itemIdentifier].className;
    var defaultRecipe = Docs.recipesByProductClass[itemClassName].FirstOrDefault(
      x => !x.isAlternative && x.isMachineRecipe
    );
    var altRecipe = Docs.recipesByProductClass[itemClassName].FirstOrDefault(
      x => context.alts.Contains(x.identifier) && x.isMachineRecipe
    );
    return (altRecipe ?? defaultRecipe!, null, amount);
  }
}
