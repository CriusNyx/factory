using SharpParse.Parsing;
using SharpParse.Util;

namespace Factory;

[ASTClass("LimitValueExp")]
public class LimitValueExpNode : LanguageNode
{
  [ASTField("ValueExp")]
  public ValueNode value;

  [ASTField("symbol")]
  public SymbolNode symbol;

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
    return new Formatting.ITree<LanguageNode>[] { value, symbol };
  }
}
