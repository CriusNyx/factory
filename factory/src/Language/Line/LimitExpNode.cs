using Factory.Util;

namespace Factory;

public class LimitExpNode : LineExpNode
{
  public LimitValueExpNode[] expressions;

  public LimitExpNode() { }

  public LimitExpNode(SourceCodeInfo sourceInfo, LimitValueExpNode[] expressions)
    : base(sourceInfo)
  {
    this.expressions = expressions;
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return expressions;
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return expressions.Map(x => x.Evaluate(ref context)).ToRecipeArgSet().With(context);
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    foreach (var exp in expressions)
    {
      exp.GetFactoryType(context);
    }
    return FactoryType.FromCSharpType(typeof(LineArgSet));
  }

  public override (string?, string?) PrintSelf()
  {
    return ("limit", null);
  }

  public override bool Equivalent(object other)
  {
    return other is LimitExpNode limit && expressions.SetEquivalent(limit.expressions);
  }
}
