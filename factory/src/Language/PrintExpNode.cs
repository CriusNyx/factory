using System.Windows.Markup;
using GenParse.Functional;
using GenParse.Util;

[ASTClass("PrintExp")]
public class PrintExpNode : ProgramExp, LanguageNode
{
  [ASTField("ValueExp*")]
  public ValueNode[] values;

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return values;
  }

  public (FactVal? value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var values = this.values.Map(x => x.Evaluate(ref context));
    foreach (var element in values)
    {
      context.standardOut.WriteLine(element);
    }

    return (null!, context);
  }
}
