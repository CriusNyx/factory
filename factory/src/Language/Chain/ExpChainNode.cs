using GenParse.Functional;
using GenParse.Util;

namespace Factory;

[ASTClass("ExpChain")]
public class ExpChainNode : ValueNode
{
  [ASTField("symbol")]
  public SymbolNode symbol;
  public ChainNode chain;

  [ASTField("ChainContinue?")]
  public ExpChainNode chainContinue;

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    if (symbol == null)
    {
      throw new NotImplementedException(
        "Attempted to evaluate a chain node that is not the head. Only the head of the chain can begin evaluation."
      );
    }
    var value = context.Resolve(symbol.Evaluate())!;
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
        return new ScopeRefVal(context, symbol.symbolName);
      }
      else
      {
        return new ObjectRefVal(owner, chain.GetIdentifier());
      }
    }
    else
    {
      if (symbol != null)
      {
        return chainContinue._GetReference(context, context.Resolve(symbol.Evaluate())!);
      }
      else
      {
        return chainContinue._GetReference(context, chain.Evaluate(owner.NotNull(), context));
      }
    }
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { symbol, chain, chainContinue }.FilterDefined();
  }

  public FactoryType ComputeRef(TypeContext context)
  {
    if (symbol != null)
    {
      var symType = new ReferenceType(symbol.symbolName, symbol);
      symbol.OverrideType(symType.ResolveType(context));
      if (chainContinue != null)
      {
        context.Push(symType);
        var result = chainContinue.GetFactoryType(context);
        context.Pop();
        return result;
      }
      return symType;
    }
    else
    {
      var chainType = chain.GetFactoryType(context);
      if (chainContinue != null)
      {
        context.Push(chainType);
        var result = chainContinue.GetFactoryType(context);
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

  public void SetAssignType(FactoryType factoryType)
  {
    if (factoryType.IsEmpty())
    {
      OverrideType(factoryType);
    }
    if (chainContinue != null)
    {
      chainContinue.SetAssignType(factoryType);
    }
    else
    {
      symbol.OverrideType(factoryType);
    }
  }
}
