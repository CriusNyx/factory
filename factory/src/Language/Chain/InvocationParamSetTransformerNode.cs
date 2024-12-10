using GenParse.Functional;

[ASTClass("InvocationParamSet")]
public class InvocationParamSetTransformerNode : ASTTransformer
{
  [ASTField("InvocationParam*")]
  public ValueNode[] invocationParams;

  [ASTField("FinalInvocationParam")]
  public ValueNode finalInvocationParam;

  public object Transform()
  {
    var invocationParams = this.invocationParams ?? new ValueNode[] { };
    return finalInvocationParam == null
      ? invocationParams
      : invocationParams.Push(finalInvocationParam);
  }
}
