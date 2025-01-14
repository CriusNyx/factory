using System.Text;
using GenParse.Functional;
using GenParse.Util;

namespace Factory;

public class RecipeSolution : FactVal
{
  public readonly RecipeSearchRequest request;
  public readonly RecipeSearchNode root;
  public readonly RecipeBalance recipeBalance;

  [ExposeMember("Recipe")]
  public RecipeValue Recipe => request.recipe;

  [ExposeMember("Total")]
  public NumVal Output => new NumVal(root.productionQuantity);

  public RecipeSolution(RecipeSearchRequest request, RecipeSearchNode root)
  {
    this.request = request;
    this.root = root;
    recipeBalance = RecipeBalance.Create(root);
  }

  public override string ToString()
  {
    var tallyArgs = request.recipe.arguments.tallyVals;

    var inlineTallys = tallyArgs.Filter(x => x.inline);
    var outlineTallys = tallyArgs.Filter(x => !x.inline);

    List<string[]> lines = new List<string[]>();

    // Recipe Name
    lines.Add(new string[] { request.recipe.recipeName }.Push(inlineTallys.Map(x => x.identifier)));

    // Blank line
    lines.Add(new string[] { "" }.Push(inlineTallys.Map(_ => "")));

    // Recursively process recipe
    RecipeSearchNode.Crawl(
      root,
      (node, depth) =>
      {
        var indentString = Formatting.TreeIndent(depth);
        lines.Add(
          new string[] { $"{indentString}{node}" }.Push(
            inlineTallys.Map((x) => ProcessInlineTally(node, x))
          )
        );
      }
    );

    // Place inline tally at bottom.
    if (inlineTallys.Length > 0)
    {
      lines.Add(new string[] { "" }.Push(inlineTallys.Map(_ => "")));
      lines.Add(
        new string[] { "Totals" }.Push(inlineTallys.Map(x => TotalTally(x).ToString("0.###")))
      );
    }

    StringBuilder builder = new StringBuilder();
    builder.AppendLine(
      Formatting.PrintGrid(
        lines.ToArray(),
        eol: inlineTallys.Length > 0 ? "  |" : "",
        alignRight: new bool[] { false }.Push(inlineTallys.Map(_ => true))
      )
    );

    if (outlineTallys.Length > 0)
    {
      builder.AppendLine($"\n{string.Join("\n", outlineTallys.Map(ProcessTally))}");
    }

    return builder.ToString();
  }

  private string ProcessInlineTally(RecipeSearchNode node, TallyVal tallyVal)
  {
    var ident = node.item.identifier ?? node.nodeName;
    if (ident == tallyVal.identifier)
    {
      return node.quantity.ToString("0.###");
    }
    return "";
  }

  public decimal TotalTally(TallyVal tallyVal) => TotalTally(tallyVal.identifier);

  [ExposeMember("Tally")]
  public NumVal TotalTally(FactVal arg)
  {
    string identifier;
    if (arg is StringVal stringVal)
    {
      identifier = stringVal.value;
    }
    else if (arg is Recipe recipe)
    {
      identifier = recipe.primaryProduct.identifier;
    }
    else
    {
      throw new ArgumentException($"Unrecognized argument type {arg.GetType()}");
    }
    return new NumVal(Math.Abs(TotalTally(identifier)));
  }

  private decimal TotalTally(string identifier)
  {
    decimal count = 0;
    root.Crawl(
      (x) => x.children,
      (x) =>
      {
        if (x.item.identifier == identifier)
        {
          count += x.quantity;
        }
      }
    );
    return count;
  }

  private string ProcessTally(TallyVal tallyVal)
  {
    return $"{tallyVal.identifier} {TotalTally(tallyVal):0.###}";
  }
}
