using SharpParse.Functional;
using SharpParse.Parsing;
using SharpParse.Util;

namespace Factory;

[ASTClass("OutExp")]
public class OutExpNode : RecipeExpNode
{
  [ASTField("symbol*")]
  public SymbolNode[] symbols;

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
}
