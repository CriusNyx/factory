using Factory;
using SharpParse.Functional;
using SharpParse.Util;

public class TermNode : ValueNode, ASTSimplifier
{
  public ValueNode factor;
  public FactorChainNode[] factorChian;

  public TermNode() { }

  public TermNode(SourceCodeInfo sourceInfo, ValueNode factor, FactorChainNode[] factorChain)
    : base(sourceInfo)
  {
    this.factor = factor;
    this.factorChian = factorChain;
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    foreach (var element in factorChian)
    {
      element.GetFactoryType(context);
    }
    return factor.GetFactoryType(context);
  }

  public override (FactVal value, Factory.ExecutionContext context) Evaluate(
    Factory.ExecutionContext context
  )
  {
    var value = factor.Evaluate(ref context);
    if (value is NumVal numVal)
    {
      foreach (var chain in factorChian)
      {
        numVal = chain.Evaluate(numVal, ref context);
      }
      return numVal.With(context);
    }
    else
    {
      if (factorChian.Length != 0)
      {
        throw new InvalidOperationException();
      }
      return value.With(context);
    }
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [factor, .. factorChian];
  }

  public bool TrySimplify(out object result)
  {
    result = null!;
    if (factorChian.Length == 0)
    {
      result = factor;
      return true;
    }
    return false;
  }

  public override bool Equivalent(object other)
  {
    return other is TermNode termNode
      && factor.Equivalent(termNode.factor)
      && factorChian.SetEquivalent(termNode.factorChian);
  }
}
