using System.Text;
using GenParse.Functional;
using GenParse.Util;

public class RecipeSearchResult : FactVal
{
  public readonly RecipeSearchRequest request;
  public readonly RecipeSearchNode root;

  public RecipeSearchResult(RecipeSearchRequest request, RecipeSearchNode root)
  {
    this.request = request;
    this.root = root;
  }

  public override string ToString()
  {
    var tallyArgs = request.recipe.arguments
      .Map(
        x =>
          x is TypedFactVal typedFactVal && typedFactVal.value is TallyVal tallyVal
            ? tallyVal
            : null!
      )
      .array.FilterDefined()
      .ToTypedArray<TallyVal>();

    var inlineTallys = tallyArgs.Filter(x => x.inline);
    var outlineTallys = tallyArgs.Filter(x => !x.inline);

    List<string[]> lines = new List<string[]>();

    lines.Add(new string[] { request.recipe.recipeName }.Push(inlineTallys.Map(x => x.symbol)));
    lines.Add(new string[] { "" }.Push(inlineTallys.Map(_ => "")));

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

    lines.Add(new string[] { "" }.Push(inlineTallys.Map(_ => "")));
    lines.Add(new string[] { "" }.Push(inlineTallys.Map(_ => "")));
    lines.Add(new string[] { "Totals" }.Push(inlineTallys.Map(x => TotalTally(x).ToString())));

    StringBuilder builder = new StringBuilder();
    builder.AppendLine(
      Formatting.PrintGrid(
        lines.ToArray(),
        eol: "  |",
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
    if (ident == tallyVal.symbol)
    {
      return node.quantity.ToString();
    }
    return "";
  }

  private decimal TotalTally(TallyVal tallyVal)
  {
    decimal count = 0;
    root.Crawl(
      (x) => x.children,
      (x) =>
      {
        if (x.item.identifier == tallyVal.symbol)
        {
          count += x.quantity;
        }
      }
    );
    return count;
  }

  private string ProcessTally(TallyVal tallyVal)
  {
    return $"{tallyVal.symbol} {TotalTally(tallyVal)}";
  }
}
