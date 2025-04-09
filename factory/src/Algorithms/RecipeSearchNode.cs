using SharpParse.Functional;
using SharpParse.Util;

namespace Factory;

public class RecipeSearchNode
{
  public readonly decimal quantity;
  public decimal productionQuantity => quantity * (recipe?.product?.FirstOrDefault()?.Amount ?? 1);
  public readonly string nodeName;
  public readonly Recipe recipe;
  public readonly Item item;
  public readonly RecipeSearchNode[] children;

  public RecipeSearchNode(Recipe recipe, decimal quantity, RecipeSearchNode[] children, Item item)
    : this(quantity, null, recipe, item, children)
  {
    this.quantity = quantity;
    this.recipe = recipe;
    this.children = children;
    this.item = item;
  }

  public RecipeSearchNode(string nodeName, decimal quantity, Item item)
    : this(quantity, nodeName, null!, item, new RecipeSearchNode[] { }) { }

  public RecipeSearchNode(
    decimal quantity,
    string? nodeName,
    Recipe? recipe,
    Item? item,
    RecipeSearchNode[] children
  )
  {
    this.quantity = quantity;
    this.nodeName = nodeName!;
    this.recipe = recipe!;
    this.item = item!;
    this.children = children;
  }

  public override string ToString()
  {
    return $"{FormatQuantity()} {recipe?.identifier ?? nodeName}";
  }

  public string PrintTree()
  {
    return Formatting.PrintTree(this, x => x.ToString(), x => x.children);
  }

  public string FormatQuantity()
  {
    return productionQuantity.ToString("0.###").Replace("-", "(-)");
  }

  public static void Crawl(RecipeSearchNode root, Action<RecipeSearchNode, int> visitor)
  {
    root.Crawl(0, (x, y) => (x.children, y + 1), visitor);
  }
}
