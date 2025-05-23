using SharpParse.Functional;
using SharpParse.Parsing;
using SharpParse.Util;

namespace Factory;

[ASTClass("SpreadExp")]
public class SpreadExpNode : RecipeExpNode
{
  [ASTField("symbol")]
  public SymbolNode symbol;

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var target = context.Resolve(symbol.Evaluate());
    var spreadMethod = target?.GetType().GetFactorySpreadMethod();
    return (spreadMethod?.Invoke(target, new object[] { }) as FactVal).NotNull().With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { symbol };
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Complex);
  }

  public override (string?, string?) PrintSelf()
  {
    return ("...", null);
  }
}
