using Factory.Util;

namespace Factory;

public class NumberLiteralNode : LiteralNode
{
  public NumberLiteralNode() { }

  public NumberLiteralNode(SourceCodeInfo sourceCodeInfo)
    : base(sourceCodeInfo) { }

  protected override FactoryType CalculateType(TypeContext context)
  {
    return FactoryType.FromCSharpType(typeof(NumVal));
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return decimal.Parse(Source).ToNumVal().With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [];
  }

  public override string ToString()
  {
    return $"{base.ToString()} {Source}";
  }

  public override (string?, string?) PrintSelf()
  {
    return (Source, null);
  }

  public override bool Equivalent(object other)
  {
    return other is NumberLiteralNode lit && Source == lit.Source;
  }
}
