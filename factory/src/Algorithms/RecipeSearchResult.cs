using System.Text;
using GenParse.Functional;

public class RecipeSearchResult
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
    StringBuilder builder = new StringBuilder();

    builder.AppendLine(root.ToString());

    if (request.args.Any(x => x is RecipeSearchRequestTallyArg))
    {
      builder.AppendLine(
        $"\n{string.Join("\n", request.args.FilterByType<RecipeSearchRequestArg, RecipeSearchRequestTallyArg>().Map(ProcessTally))}"
      );
    }

    return builder.ToString();
  }

  public string ProcessTally(RecipeSearchRequestTallyArg tallyArg)
  {
    decimal count = 0;
    root.Crawl(
      (x) => x.children,
      (x) =>
      {
        if (x.item.identifier == tallyArg.itemIdentifier)
        {
          count += x.quantity;
        }
      }
    );

    return $"{tallyArg.itemIdentifier} {count}";
  }
}
