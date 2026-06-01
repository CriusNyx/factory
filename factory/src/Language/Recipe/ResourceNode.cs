using Factory;
using Factory.Util;

public class ResourceNode : StatementNode
{
  public SymbolNode Symbol;

  public ResourceNode(SourceCodeInfo sourceInfo, SymbolNode symbol)
    : base(sourceInfo)
  {
    Symbol = symbol;
  }

  protected override void CalculateStaticType(TypeContext context)
  {
    context.SetGlobalType(Symbol.symbolName, FactoryType.FromCSharpType(typeof(ResourceVal)));
    base.CalculateStaticType(context);
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    if (context.GetType(Symbol.Source) is CSharpType csType)
    {
      if (csType.type == typeof(RecipeVal) || csType.type == typeof(RecipeAndResourceVal))
      {
        return FactoryType.FromCSharpType(typeof(RecipeAndResourceVal));
      }
    }
    return FactoryType.FromCSharpType(typeof(ResourceVal));
  }

  public override (FactVal value, Factory.ExecutionContext context) Evaluate(
    Factory.ExecutionContext context
  )
  {
    var symVal = Symbol.Evaluate(ref context).AsSymbolVal();
    var output = new ResourceVal(Symbol.Source);
    if (context.Resolve(symVal) is HasRecipe recVal)
    {
      return new RecipeAndResourceVal(output, recVal.Recipe).With(context);
    }
    return output.With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [Symbol];
  }

  public override bool Equivalent(object other)
  {
    return other is ResourceNode rec && Symbol.Equivalent(rec.Symbol);
  }
}
