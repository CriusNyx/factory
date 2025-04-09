using SharpParse.Functional;
using SharpParse.Parsing;
using SharpParse.Util;

namespace Factory;

[ASTClass("stringLiteral")]
public class StringLiteralNode : LiteralNode
{
  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [];
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var sourceCode = astNode.SourceCode();
    var stringSegment = sourceCode.Substring(1, sourceCode.Length - 2);
    return new StringVal(stringSegment).With(context);
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.String);
  }

  public override (string?, string?) PrintSelf()
  {
    return (astNode.SourceCode(), null);
  }
}
