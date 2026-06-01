using Factory.Util;

namespace Factory;

public class PrintStatementNode : StatementNode
{
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

  protected override FactoryType CalculateType(TypeContext context)
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

  public override bool Equivalent(object other)
  {
    return other is PrintStatementNode print && values.SetEquivalent(print.values);
  }

  public static PrintStatementNode Create(SourceCodeInfo sourceInfo, ValueNode[] values)
  {
    var output = new PrintStatementNode();
    output.SetSourceInfo(sourceInfo);
    output.values = values;
    return output;
  }
}
