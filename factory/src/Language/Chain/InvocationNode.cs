using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("Invocation")]
public class InvocationNode(ASTNode<FactoryLexon> astNode) : LanguageNode, ChainNode
{
  private ASTNode<FactoryLexon> _astNode = astNode;
  public ASTNode<FactoryLexon> astNode => _astNode;

  [ASTField("InvocationParamSet")]
  public ValueNode[] parameters;

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return parameters;
  }

  public FactVal Evaluate(FactVal target, ExecutionContext context)
  {
    var invocationParams = parameters.Map(x => x.Evaluate(ref context)).ToArrayVal();

    if (target is IFunc func)
    {
      return func.Invoke(invocationParams);
    }
    return null!;
  }

  public string GetIdentifier()
  {
    throw new Exception("Invocation nodes cannot be converted to identifiers.");
  }
}
