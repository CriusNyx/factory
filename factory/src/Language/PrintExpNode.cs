using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("PrintExp")]
public class PrintExpNode : ProgramExp
{
  public override ASTNode<FactoryLexon> astNode => throw new NotImplementedException();

  [AST]
  public ASTNode<FactoryLexon> _astNode { get; set; }

  [ASTField("PrintExpChain")]
  public ValueNode[] values;

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return values;
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var values = this.values.Map(x => x.Evaluate(ref context));
    foreach (var element in values)
    {
      var elementString = element.ToString();
      context.standardOut.WriteLine(element);
    }

    return (null!, context);
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    foreach (var value in values)
    {
      value.CalculateType(context);
    }
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Void);
  }
}
