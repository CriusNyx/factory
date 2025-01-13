using Factory;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("Factor")]
public class FactorNode : LanguageNode, ValueNode
{
  [ASTField("minus?")]
  public bool negative;

  [ASTField("Primitive")]
  public ValueNode primitive;

  [AST]
  public ASTNode<FactoryLexon> astNode { get; set; }

  public FactoryType CalculateType(TypeContext context)
  {
    return primitive.CalculateType(context);
  }

  public (FactVal value, Factory.ExecutionContext context) Evaluate(
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

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [primitive];
  }
}
