using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("ExpChain")]
public class ExpChainNode : ValueNode
{
  [ASTField("symbol")]
  public SymbolNode initialSymbol;
  public ChainNode chain;

  [ASTField("ChainContinue?")]
  public ExpChainNode chainContinue;

  public override ASTNode<FactoryLexon> astNode => _astNode;

  [AST]
  public ASTNode<FactoryLexon> _astNode { get; set; }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
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

  public RefVal GetReference(ExecutionContext context)
  {
    return _GetReference(context, null);
  }

  private RefVal _GetReference(ExecutionContext context, FactVal? owner)
  {
    if (chainContinue == null)
    {
      if (owner == null)
      {
        return new ScopeRefVal(context, initialSymbol.symbolName);
      }
      else
      {
        return new ObjectRefVal(owner, chain.GetIdentifier());
      }
    }
    else
    {
      if (initialSymbol != null)
      {
        return chainContinue._GetReference(context, context.Resolve(initialSymbol.Evaluate())!);
      }
      else
      {
        return chainContinue._GetReference(context, chain.Evaluate(owner.NotNull(), context));
      }
    }
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[]
    {
      initialSymbol,
      chain,
      chainContinue,
    }.FilterDefined();
  }

  public FactoryType ComputeRef(TypeContext context)
  {
    if (initialSymbol != null)
    {
      var symType = new ReferenceType(initialSymbol.symbolName);
      if (chainContinue != null)
      {
        context.Push(symType);
        var result = chainContinue.CalculateType(context);
        context.Pop();
        return result;
      }
      return symType;
    }
    else
    {
      var chainType = chain.CalculateType(context);
      if (chainContinue != null)
      {
        context.Push(chainType);
        var result = chainContinue.CalculateType(context);
        context.Pop();
        return result;
      }
      return chainType;
    }
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return ComputeRef(context).ResolveType(context);
  }
}
