using Factory.Util;

namespace Factory;

public class StringLiteralNode : LiteralNode
{
  public string StringValue => Source.Substring(1, Source.Length - 2);

  public StringLiteralNode() { }

  public StringLiteralNode(SourceCodeInfo sourceInfo)
    : base(sourceInfo) { }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [];
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var sourceCode = Source;
    var stringSegment = sourceCode.Substring(1, sourceCode.Length - 2);
    return new StringVal(stringSegment).With(context);
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.String);
  }

  public override (string?, string?) PrintSelf()
  {
    return (Source, null);
  }

  public override bool Equivalent(object other)
  {
    return other is StringLiteralNode lit && Source == lit.Source;
  }
}
