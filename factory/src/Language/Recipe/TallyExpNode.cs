using Factory.Util;

namespace Factory;

public class TallyExpNode : LineExpNode
{
  public bool inline;
  public SymbolNode[] symbols;

  public TallyExpNode() { }

  public TallyExpNode(SourceCodeInfo sourceInfo, bool inline, SymbolNode[] symbols)
    : base(sourceInfo)
  {
    this.inline = inline;
    this.symbols = symbols;
  }

  public override string ToString()
  {
    return $"{base.ToString()} inline={inline}";
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return symbols.Map(x => new TallyVal(x.symbolName, inline)).ToRecipeArgSet().With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return symbols;
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return FactoryType.FromCSharpType(typeof(RecipeArgSet));
  }

  public override (string?, string?) PrintSelf()
  {
    string output = "tally";
    if (inline)
    {
      output += " inline";
    }
    return (output, null);
  }

  public override bool Equivalent(object other)
  {
    return other is TallyExpNode tally
      && inline == tally.inline
      && symbols.SetEquivalent(tally.symbols);
  }
}
