using GenParse.Functional;
using GenParse.Util;

[ASTClass("AssignExp")]
public class AssignExpNode : ProgramExp, LanguageNode
{
  [ASTField("symbol")]
  public SymbolNode symbol;

  [ASTField("ValueExp")]
  public ValueNode value;

  public (FactVal? value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var result = value.Evaluate(ref context);
    context.GlobalValues[symbol.symbolName] = result!;
    return result.With(context);
  }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { symbol, value };
  }
}
