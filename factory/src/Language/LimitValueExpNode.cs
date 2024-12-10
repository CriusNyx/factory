using GenParse.Functional;
using GenParse.Util;

[ASTClass("LimitValueExp")]
public class LimitValueExpNode : LanguageNode
{
  [ASTField("ValueExp")]
  public ValueNode value;

  [ASTField("symbol")]
  public SymbolNode symbol;

  public FactVal Evaluate(ref ExecutionContext context)
  {
    FactVal result;
    (result, context) = Evaluate(context);
    return result;
  }

  public (FactVal factVal, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return new TypedFactVal(
      ValType.limit,
      new PairVal(value.Evaluate(ref context), symbol.Evaluate(ref context))
    ).With(context);
  }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { value, symbol };
  }
}
