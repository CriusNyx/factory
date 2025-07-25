using SharpParse.Functional;
using SharpParse.Util;

namespace Factory;

public class InExpNode : LineExpNode
{
  public SymbolNode[] symbols;

  public InExpNode() { }

  public InExpNode(SourceCodeInfo sourceInfo, SymbolNode[] symbols)
    : base(sourceInfo)
  {
    this.symbols = symbols;
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return symbols.Map(x => new InVal(x.symbolName)).ToRecipeArgSet().With(context);
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
    return ("in", null);
  }

  public override bool Equivalent(object other)
  {
    return other is InExpNode exp && symbols.SetEquivalent(exp.symbols);
  }
}
