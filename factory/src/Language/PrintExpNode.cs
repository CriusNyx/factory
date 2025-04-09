using SharpParse.Functional;
using SharpParse.Util;

namespace Factory;

[ASTClass("PrintExp")]
public class PrintExpNode : ProgramExp
{
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
      value.GetFactoryType(context);
    }
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Void);
  }

  public override (string?, string?) PrintSelf()
  {
    return ("print", null);
  }
}
