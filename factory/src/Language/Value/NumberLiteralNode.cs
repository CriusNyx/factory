using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("numberLiteral")]
public class NumberLiteralNode(ASTNode<FactoryLexon> astNode) : LiteralNode
{
  ASTNode<FactoryLexon> _astNode = astNode;

  public override ASTNode<FactoryLexon> astNode => _astNode;

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
    return new Formatting.ITree<LanguageNode>[] { };
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
