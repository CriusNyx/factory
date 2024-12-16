using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("stringLiteral")]
public class StringLiteralNode : LanguageNode, ValueNode
{
  [AST]
  public ASTNode<FactoryLexon> astNode { get; set; }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [];
  }

  public (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var sourceCode = astNode.SourceCode();
    var stringSegment = sourceCode.Substring(1, sourceCode.Length - 2);
    return new StringVal(stringSegment).With(context);
  }

  public FactoryType CalculateType(TypeContext context)
  {
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.String);
  }
}
