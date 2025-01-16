using Factory;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("Factor")]
public class FactorNode : ValueNode, ASTSimplifier
{
  [ASTField("minus?")]
  public bool negative;

  [ASTField("Primitive")]
  public ValueNode primitive;

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

  public bool TrySimplify(out object result)
  {
    result = null!;
    if (!negative)
    {
      result = primitive;
      return true;
    }
    return false;
  }
}
