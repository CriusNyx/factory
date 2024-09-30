using GenParse.Functional;

namespace GenParse.Parsing;

public class ProductionSet<LexonType>
{
  public readonly ProductionRule<LexonType>[] rules;

  public ProductionSet(ProductionRule<LexonType>[] rules)
  {
    this.rules = rules;
  }

  public override string ToString()
  {
    return string.Join("\n", rules.Map(x => x.ToString()));
  }
}
