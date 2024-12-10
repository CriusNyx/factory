using System.Reflection;
using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("ExpChain")]
public class ExpChainNode : LanguageNode, ValueNode
{
  [ASTField("symbol")]
  public SymbolNode initialSymbol;
  public ChainNode chain;

  [ASTField("ChainContinue?")]
  public ExpChainNode chainContinue;

  public ASTNode<FactoryLexon> astNode => throw new NotImplementedException();

  public (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    if (initialSymbol == null)
    {
      throw new NotImplementedException(
        "Attempted to evaluate a chain node that is not the head. Only the head of the chain can begin evaluation."
      );
    }
    var value = context.Resolve(initialSymbol.Evaluate())!;
    if (chainContinue != null)
    {
      return chainContinue.Continue(value, context).With(context);
    }
    else
    {
      return value.With(context)!;
    }
  }

  private FactVal Continue(FactVal value, ExecutionContext context)
  {
    value = chain.Evaluate(value, context);
    if (chainContinue == null)
    {
      return value;
    }
    else
    {
      return chainContinue.Continue(value, context);
    }
  }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[]
    {
      initialSymbol,
      chain,
      chainContinue,
    }.FilterDefined();
  }
}
