namespace GenParse.Parsing;

public class ParseContext<LexonType>
{
  public readonly IReadOnlyDictionary<string, ProductionSet<LexonType>> productionSets;

  public ParseContext(IReadOnlyDictionary<string, ProductionSet<LexonType>> productionSets)
  {
    this.productionSets = productionSets;
  }

  public override string ToString()
  {
    return "Grammar:\n\n"
      + string.Join("\n\n", productionSets.Select((pair) => $"{pair.Key}:\n{pair.Value}"));
  }
}
