using SharpParse.Functional;

namespace Factory;

/// <summary>
/// The balance of a recipe.
/// </summary>
public class RecipeBalance
{
  /// <summary>
  /// Balance elements
  /// </summary>
  public readonly RecipeBalanceElement[] elements;

  public RecipeBalance(RecipeBalanceElement[] elements)
  {
    this.elements = elements;
  }

  public static RecipeBalance Create(RecipeSearchNode root)
  {
    List<RecipeBalanceElement> elements = new List<RecipeBalanceElement>()
    {
      new RecipeBalanceElement(root.item.identifier, root.quantity),
    };

    foreach (var child in root.children)
    {
      child.Crawl(
        node => node.children,
        node =>
        {
          if (node.children.Length == 0)
          {
            var identifier = node.item.identifier;
            elements.ReplaceOrAdd(
              balanceElement => balanceElement.identifier == identifier,
              balanceElement =>
                balanceElement.Clone(identifier, balanceElement.quantity - node.quantity)
            );
          }
        }
      );
    }

    return new RecipeBalance(elements.ToArray());
  }

  public decimal GetValueForIdentifier(string identifier)
  {
    return Math.Abs(elements.FirstOrDefault(x => x.identifier == identifier).quantity);
  }
}

public struct RecipeBalanceElement
{
  public readonly string identifier;
  public readonly decimal quantity;

  public RecipeBalanceElement(string identifier, decimal quantity)
  {
    this.identifier = identifier;
    this.quantity = quantity;
  }

  public RecipeBalanceElement Clone(string? identifier = null, decimal? quantity = null)
  {
    return new RecipeBalanceElement(identifier ?? this.identifier, quantity ?? this.quantity);
  }
}
