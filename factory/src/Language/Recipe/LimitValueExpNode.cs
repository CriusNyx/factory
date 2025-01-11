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
    value.CalculateType(context);
    return FactoryType.FromCSharpType(typeof(LimitVal));
  }

  public LimitVal Evaluate(ref ExecutionContext context)
  {
    return new LimitVal(symbol.symbolName, (value.Evaluate(ref context) as NumVal)!);
  }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { value, symbol };
  }
}
