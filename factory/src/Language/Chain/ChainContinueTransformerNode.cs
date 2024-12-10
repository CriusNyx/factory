[ASTClass("ChainContinue")]
public class ChainContinueTransformerNode : ASTTransformer
{
  [ASTField("Chain")]
  public ChainNode chainNode;

  [ASTField("ChainContinue?")]
  public ExpChainNode chainContinue;

  public object Transform()
  {
    var outputNode = new ExpChainNode();
    outputNode.chain = chainNode;
    outputNode.chainContinue = chainContinue;
    return outputNode;
  }
}
