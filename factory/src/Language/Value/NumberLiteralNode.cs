using SharpParse.Functional;
using SharpParse.Parsing;
using SharpParse.Util;

namespace Factory;

[ASTClass("numberLiteral")]
public class NumberLiteralNode : LiteralNode
{
  public override FactoryType CalculateType(TypeContext context)
  {
    return FactoryType.FromCSharpType(typeof(NumVal));
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return decimal.Parse(astNode.SourceCode()).ToNumVal().With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [];
  }

  public override string ToString()
  {
    return $"{base.ToString()} {astNode.SourceCode()}";
  }

  public override (string?, string?) PrintSelf()
  {
    return (astNode.SourceCode(), null);
  }
}
