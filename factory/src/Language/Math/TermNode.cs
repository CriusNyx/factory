using Factory;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("Term")]
public class TermNode : ValueNode, ASTSimplifier
{
  [ASTField("Factor")]
  public ValueNode factor;

  [ASTField("FactorChain*")]
  public FactorChainNode[] factorChian;

  public override ASTNode<FactoryLexon> astNode => _astNode;

  [AST]
  public ASTNode<FactoryLexon> _astNode { get; set; }

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
}
