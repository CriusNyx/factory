using Factory;
using GenParse.Parsing;

[ASTClass("PrintExpChain")]
public class PrintExpChainTransformerNode : ASTTransformer
{
  public PrintExpChainTransformerNode() { }

  [ASTField("ValueChain")]
  public ValueChainNode valueChainNode;

  public object Transform()
  {
    List<ValueNode> values = new List<ValueNode>();
    TraverseValueChainNode(values, valueChainNode);
    return values.ToArray();
  }

  private void TraverseValueChainNode(List<ValueNode> list, ValueChainNode? chainNode)
  {
    if (chainNode != null)
    {
      if (chainNode.valueNode != null)
      {
        list.Add(chainNode.valueNode);
      }
      TraverseValueChainNode(list, chainNode.valueChainNode);
    }
  }
}
