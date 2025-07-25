namespace Factory;

public abstract class ValueNode : LanguageNode
{
  public ValueNode() { }

  public ValueNode(SourceCodeInfo sourceInfo)
    : base(sourceInfo) { }

  public abstract (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context);
}

public static class ValueNodeExtensions
{
  public static FactVal Evaluate(this ValueNode valueNode, ref ExecutionContext context)
  {
    FactVal result;
    (result, context) = valueNode.Evaluate(context);
    return result;
  }
}
