using SharpParse.Util;

namespace Factory;

public class LimitValueExpNode : LanguageNode
{
  public ValueNode value;
  public SymbolNode symbol;

  public LimitValueExpNode() { }

  public LimitValueExpNode(SourceCodeInfo sourceInfo, ValueNode value, SymbolNode symbol)
    : base(sourceInfo)
  {
    this.value = value;
    this.symbol = symbol;
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    value.GetFactoryType(context);
    symbol.GetFactoryType(context);
    return FactoryType.FromCSharpType(typeof(LimitVal));
  }

  public LimitVal Evaluate(ref ExecutionContext context)
  {
    return new LimitVal(symbol.symbolName, (value.Evaluate(ref context) as NumVal)!);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [value, symbol];
  }

  public override bool Equivalent(object other)
  {
    return other is LimitValueExpNode val
      && value.Equivalent(val.value)
      && symbol.Equivalent(val.symbol);
  }
}
