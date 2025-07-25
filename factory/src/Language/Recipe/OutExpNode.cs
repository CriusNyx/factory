using Factory.Util;

namespace Factory;

public class OutExpNode : LineExpNode
{
  public SymbolNode[] symbols;

  public OutExpNode() { }

  public OutExpNode(SourceCodeInfo sourceInfo, SymbolNode[] symbols)
    : base(sourceInfo)
  {
    this.symbols = symbols;
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return symbols.Map(x => new OutVal(x.symbolName)).ToRecipeArgSet().With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren() => symbols;

  public override FactoryType CalculateType(TypeContext context)
  {
    foreach (var sym in symbols)
    {
      sym.GetFactoryType(context);
    }
    return FactoryType.FromCSharpType(typeof(RecipeArgSet));
  }

  public override (string?, string?) PrintSelf()
  {
    return ("out", null);
  }

  public override bool Equivalent(object other)
  {
    return other is OutExpNode exp && symbols.SetEquivalent(exp.symbols);
  }
}
