using Factory;
using Factory.Util;

public class MathExpNode : ValueNode
{
  public ValueNode term;
  public TermChainNode[] termChain;

  public MathExpNode() { }

  public MathExpNode(SourceCodeInfo sourceInfo, ValueNode term, TermChainNode[] termChain)
    : base(sourceInfo)
  {
    this.term = term;
    this.termChain = termChain;
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    foreach (var element in termChain)
    {
      element.GetFactoryType(context);
    }
    return term.GetFactoryType(context);
  }

  public override (FactVal value, Factory.ExecutionContext context) Evaluate(
    Factory.ExecutionContext context
  )
  {
    var value = term.Evaluate(ref context);
    if (value is NumVal numVal)
    {
      foreach (var chain in termChain)
      {
        numVal = chain.Evaluate(numVal, ref context);
      }
      return numVal.With(context);
    }
    else
    {
      if (termChain.Length != 0)
      {
        throw new InvalidOperationException();
      }
      return value.With(context);
    }
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [term, .. termChain];
  }

  public override bool TrySimplify(out ValueNode result)
  {
    result = null!;
    if (termChain.Length == 0)
    {
      result = term;
      return true;
    }
    return false;
  }

  public override bool Equivalent(object other)
  {
    return other is MathExpNode math
      && term.Equivalent(math.term)
      && termChain.SetEquivalent(math.termChain);
  }
}
