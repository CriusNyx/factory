using Factory;
using GenParse.Util;

[ASTClass("FactorChain")]
public class FactorChainNode : LanguageNode
{
  [ASTField("FactorOperation")]
  public OperationNode operation;

  [ASTField("Factor")]
  public FactorNode factor;

  public FactoryType CalculateType(TypeContext context)
  {
    return factor.CalculateType(context);
  }

  public NumVal Evaluate(NumVal leftOperand, ref Factory.ExecutionContext context)
  {
    var thisVal = factor.Evaluate(ref context).To<NumVal>();
    switch (operation.operation)
    {
      case "*":
        return new NumVal(leftOperand.value * thisVal.value);
      case "/":
        return new NumVal(leftOperand.value / thisVal.value);
      case "%":
        return new NumVal(leftOperand.value % thisVal.value);
      default:
        throw new NotImplementedException();
    }
  }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [operation, factor];
  }
}