using Factory;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("MathExp")]
public class MathExpNode : ValueNode, ASTSimplifier
{
  [ASTField("Term")]
  public ValueNode term;

  [ASTField("TermChain*")]
  public TermChainNode[] termChain;

  public override ASTNode<FactoryLexon> astNode => _astNode;

  [AST]
  public ASTNode<FactoryLexon> _astNode { get; set; }

  public override FactoryType CalculateType(TypeContext context)
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

  public bool TrySimplify(out object result)
  {
    result = null!;
    if (termChain.Length == 0)
    {
      result = term;
      return true;
    }
    return false;
  }
}
