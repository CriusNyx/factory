namespace Factory;

public abstract class ValueNode : LanguageNode, Simplifier<ValueNode>
{
  public ValueNode() { }

  public ValueNode(SourceCodeInfo sourceInfo)
    : base(sourceInfo) { }

  public abstract (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context);

  public virtual bool TrySimplify(out ValueNode? value)
  {
    value = null;
    return false;
  }
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
