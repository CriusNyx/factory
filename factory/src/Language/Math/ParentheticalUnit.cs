using Factory;
using Factory.Util;

public class ParentheticalUnit : ValueNode
{
  public ValueNode value;

  public ParentheticalUnit() { }

  public ParentheticalUnit(SourceCodeInfo info, ValueNode value)
    : base(info)
  {
    this.value = value;
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    return value.GetFactoryType(context);
  }

  public override (FactVal value, Factory.ExecutionContext context) Evaluate(
    Factory.ExecutionContext context
  )
  {
    return value.Evaluate(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [value];
  }

  public override (string?, string?) PrintSelf()
  {
    return ("(", ")");
  }

  public override bool Equivalent(object other)
  {
    return other is ParentheticalUnit paren && value.Equivalent(paren.value);
  }
}
