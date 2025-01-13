using Factory;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("Term")]
public class TermNode : LanguageNode, ValueNode
{
  [ASTField("Factor")]
  public FactorNode factor;

  [ASTField("FactorChain*")]
  public FactorChainNode[] factorChian;

  [AST]
  public ASTNode<FactoryLexon> astNode { get; set; }

  public FactoryType CalculateType(TypeContext context)
  {
    foreach (var element in factorChian)
    {
      element.CalculateType(context);
    }
    return factor.CalculateType(context);
  }

  public (FactVal value, Factory.ExecutionContext context) Evaluate(
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

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [factor, .. factorChian];
  }
}
