using Factory;
using SharpParse.Util;

public class FactorChainNode : LanguageNode
{
  public OperationNode operation;

  public ValueNode factor;

  public FactorChainNode() { }

  public FactorChainNode(SourceCodeInfo sourceInfo, OperationNode operation, ValueNode factor)
    : base(sourceInfo)
  {
    this.operation = operation;
    this.factor = factor;
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return factor.GetFactoryType(context);
  }

  public NumVal Evaluate(NumVal leftOperand, ref Factory.ExecutionContext context)
  {
    var thisVal = factor.Evaluate(ref context).To<NumVal>();
    switch (operation.Operation)
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

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [operation, factor];
  }

  public override bool Equivalent(object other)
  {
    return other is FactorChainNode chain
      && operation.Equivalent(chain.operation)
      && factor.Equivalent(chain.factor);
  }
}
