using Factory;
using Factory.Util;

public class FactorNode : ValueNode
{
  public bool negative;
  public ValueNode primitive;

  public FactorNode() { }

  public FactorNode(SourceCodeInfo sourceInfo, bool negative, ValueNode primitive)
    : base(sourceInfo)
  {
    this.negative = negative;
    this.primitive = primitive;
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return primitive.GetFactoryType(context);
  }

  public override (FactVal value, Factory.ExecutionContext context) Evaluate(
    Factory.ExecutionContext context
  )
  {
    var output = primitive.Evaluate(ref context);
    if (negative)
    {
      return new NumVal(-output.To<NumVal>().value).With(context);
    }
    return output.With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [primitive];
  }

  public override (string?, string?) PrintSelf()
  {
    return (negative ? "-" : "", null);
  }

  public override bool TrySimplify(out ValueNode result)
  {
    result = null!;
    if (!negative)
    {
      result = primitive;
      return true;
    }
    return false;
  }

  public override bool Equivalent(object other)
  {
    return other is FactorNode factorNode
      && negative == factorNode.negative
      && primitive.Equivalent(factorNode.primitive);
  }

  public static FactorNode Create(SourceCodeInfo sourceInfo, bool negate, ValueNode primitive)
  {
    var output = new FactorNode();
    output.SetSourceInfo(sourceInfo);
    output.negative = negate;
    output.primitive = primitive;
    return output;
  }
}
