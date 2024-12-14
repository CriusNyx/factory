using GenParse.Parsing;

namespace Factory;

public interface ValueNode : LanguageNode
{
  public ASTNode<FactoryLexon> astNode { get; }

  (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context);
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
