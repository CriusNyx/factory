using Factory;
using Factory.Util;

public class TermChainNode : LanguageNode
{
  public OperationNode operation;
  public ValueNode term;

  public TermChainNode() { }

  public TermChainNode(SourceCodeInfo sourceInfo, OperationNode operation, ValueNode term)
    : base(sourceInfo)
  {
    this.operation = operation;
    this.term = term;
  }

  public NumVal Evaluate(NumVal leftOperand, ref Factory.ExecutionContext context)
  {
    var thisVal = term.Evaluate(ref context).To<NumVal>();
    switch (operation.Operation)
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
    return term.GetFactoryType(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [operation, term];
  }

  public override bool Equivalent(object other)
  {
    return other is TermChainNode chain
      && operation.Equivalent(chain.operation)
      && term.Equivalent(chain.term);
  }
}
