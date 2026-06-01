using Factory.Util;

namespace Factory;

public class AltExpNode : LineExpNode
{
  public SymbolNode[] symbols;

  public AltExpNode() { }

  public AltExpNode(SourceCodeInfo sourceInfo, SymbolNode[] symbols)
    : base(sourceInfo)
  {
    this.symbols = symbols;
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return symbols.Map(x => new AltVal(x.symbolName)).ToRecipeArgSet().With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return symbols;
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    foreach (var sym in symbols)
    {
      sym.GetFactoryType(context);
    }
    return FactoryType.FromCSharpType(typeof(LineArgSet));
  }

  public override (string?, string?) PrintSelf()
  {
    return ("alt", null);
  }

  public override bool Equivalent(object other)
  {
    return other is AltExpNode exp && symbols.SetEquivalent(exp.symbols);
  }
}
