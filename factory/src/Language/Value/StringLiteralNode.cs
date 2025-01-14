using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("stringLiteral")]
public class StringLiteralNode : ValueNode
{
  public override ASTNode<FactoryLexon> astNode => _astNode;

  [AST]
  public ASTNode<FactoryLexon> _astNode { get; set; }

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
