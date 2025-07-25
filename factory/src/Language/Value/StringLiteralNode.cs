using SharpParse.Functional;
using SharpParse.Util;

namespace Factory;

public class StringLiteralNode : LiteralNode
{
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

  public override FactoryType CalculateType(TypeContext context)
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
