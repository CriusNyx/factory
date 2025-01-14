using Factory;
using GenParse.Util;

[ASTClass("TermChain")]
public class TermChainNode : LanguageNode
{
  [ASTField("TermOperation")]
  public OperationNode operation;

  [ASTField("Term")]
  public ValueNode term;

  public NumVal Evaluate(NumVal leftOperand, ref Factory.ExecutionContext context)
  {
    var thisVal = term.Evaluate(ref context).To<NumVal>();
    switch (operation.operation)
    {
      case "+":
        return new NumVal(leftOperand.value + thisVal.value);
      case "-":
        return new NumVal(leftOperand.value - thisVal.value);
      default:
        throw new NotImplementedException();
    }
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return term.CalculateType(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [operation, term];
  }
}
