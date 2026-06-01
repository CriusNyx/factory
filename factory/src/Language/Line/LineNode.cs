using Factory.Util;

namespace Factory;

public class LineNode : StatementNode
{
  public SymbolNode name;
  public LineExpNode[] expressions;

  public LineNode() { }

  public LineNode(SourceCodeInfo sourceInfo, SymbolNode name, LineExpNode[] expressions)
    : base(sourceInfo)
  {
    this.name = name;
    this.expressions = expressions;
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    name.OverrideType(new CSharpType(typeof(LineValue)));
    name.refInfo = new RefInfo(name.symbolName);
    foreach (var expression in expressions)
    {
      expression.GetFactoryType(context);
    }
    context.SetLocalType(name.symbolName, new CSharpType(typeof(LineValue)));
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Void);
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var expressionValues = expressions.Map(x => x.Evaluate(ref context));

    var recipe = expressionValues.Reduce(
      new LineValue(name.symbolName),
      (factVal, recVal) => recVal.Amend((factVal as LineArgSet).NotNull())
    );
    context.Assign(recipe.lineName, recipe);
    return recipe.With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren() =>
    [name, .. expressions];

  public override (string?, string?) PrintSelf()
  {
    return ("recipe", null);
  }

  public override bool Equivalent(object other)
  {
    return other is LineNode line
      && name.Equivalent(line.name)
      && expressions.SetEquivalent(line.expressions);
  }
}
