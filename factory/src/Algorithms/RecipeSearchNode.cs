using GenParse.Util;

public class RecipeSearchNode
{
  public readonly decimal quantity;
  public readonly string nodeName;
  public readonly Recipe recipe;
  public readonly Item item;
  public readonly RecipeSearchNode[] children;

  public RecipeSearchNode(Recipe recipe, decimal quantity, RecipeSearchNode[] children, Item item)
  {
    this.quantity = quantity;
    this.recipe = recipe;
    this.children = children;
    this.item = item;
  }

  public RecipeSearchNode(string nodeName, decimal quantity, Item item)
  {
    this.quantity = quantity;
    this.nodeName = nodeName;
    children = new RecipeSearchNode[] { };
    this.item = item;
  }

  public override string ToString()
  {
    return Formatting.PrintTree(
      this,
      x => $"{x.FormatQuantity()} {x.recipe?.identifier ?? x.nodeName}",
      x => x.children
    );
  }

  string FormatQuantity()
  {
    return (
      quantity * (recipe?.product?.FirstOrDefault()?.amount ?? 1) / item.ComputeUIConversionRate()
    )
      .ToString()
      .Replace("-", "(-)");
  }
}
