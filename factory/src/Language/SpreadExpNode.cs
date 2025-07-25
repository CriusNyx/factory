using SharpParse.Functional;
using SharpParse.Util;

namespace Factory;

public class SpreadExpNode : LineExpNode
{
  public SymbolNode symbol;

  public SpreadExpNode() { }

  public SpreadExpNode(SourceCodeInfo sourceCodeInfo, SymbolNode symbol)
    : base(sourceCodeInfo)
  {
    this.symbol = symbol;
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var target = context.Resolve(symbol.Evaluate());
    var spreadMethod = target?.GetType().GetFactorySpreadMethod();
    return (spreadMethod?.Invoke(target, []) as FactVal).NotNull().With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [symbol];
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Complex);
  }

  public override (string?, string?) PrintSelf()
  {
    return ("...", null);
  }

  public override bool Equivalent(object other)
  {
    return other is SpreadExpNode spread && symbol.Equivalent(spread.symbol);
  }
}
