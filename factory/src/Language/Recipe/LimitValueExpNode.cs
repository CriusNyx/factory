using System.Formats.Tar;
using System.Net.Mime;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("LimitValueExp")]
public class LimitValueExpNode : LanguageNode
{
  [AST]
  public ASTNode<FactoryLexon> ast;

  [ASTField("ValueExp")]
  public ValueNode value;

  [ASTField("symbol")]
  public SymbolNode symbol;

  public FactoryType CalculateType(TypeContext context)
  {
    var valueType = value.CalculateType(context);
    if (!FactoryType.NumberType.CanAcceptValue(valueType))
    {
      var pos = value.astNode.CalculatePosition();
      context.AddError(pos.start, pos.length, $"Cannot assign {valueType.ToString()} to Number");
    }
    var symbolType = symbol.CalculateType(context);
    return new CSharpType(typeof(TypedFactVal));
  }

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
