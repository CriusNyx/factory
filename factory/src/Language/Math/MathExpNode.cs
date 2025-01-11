using Factory;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("MathExp")]
public class MathExpNode : LanguageNode, ValueNode
{
  [ASTField("Term")]
  public TermNode term;

  [ASTField("TermChain*")]
  public TermChainNode[] termChain;

  public ASTNode<FactoryLexon> astNode => throw new NotImplementedException();

  public FactoryType CalculateType(TypeContext context)
  {
    foreach (var element in termChain)
    {
      element.CalculateType(context);
    }
    return term.CalculateType(context);
  }

  public (FactVal value, Factory.ExecutionContext context) Evaluate(
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

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [term, .. termChain];
  }
}
