using Factory;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("ValueChain")]
public class ValueChainNode : LanguageNode
{
  public ValueNode valueNode;

  public ValueChainNode? valueChainNode;

  public ValueChainNode(ASTNode<FactoryLexon> astNode)
  {
    if (astNode.TryMatch(["ValueExp", "ValueChain"], out var elements))
    {
      valueNode = Transformer.Transform(elements[0]).To<ValueNode>();
      valueChainNode = Transformer.Transform(elements[1]).To<ValueChainNode>();
    }
    else if (astNode.TryMatch("ValueExp", out var valueExpNode))
    {
      valueNode = Transformer.Transform(valueExpNode).NotNull().To<ValueNode>();
      valueChainNode = null;
    }
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    var output = valueNode.GetFactoryType(context);
    valueChainNode?.GetFactoryType(context);
    return output;
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return (new LanguageNode[] { valueNode, valueChainNode! }).FilterDefined();
  }
}
