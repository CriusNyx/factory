using SharpParse.Functional;
using SharpParse.Util;

namespace Factory;

public class SymbolNode : ValueNode
{
  public string symbolName => Source;
  private RefInfo _refInfo;
  public RefInfo refInfo
  {
    get => _refInfo;
    set { _refInfo = value; }
  }

  public SymbolNode() { }

  public SymbolNode(SourceCodeInfo sourceInfo)
    : base(sourceInfo) { }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [];
  }

  public SymbolVal Evaluate()
  {
    return new SymbolVal(symbolName);
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return Evaluate().With(context);
  }

  public override string ToString() => $"Symbol {Source}";

  public override FactoryType CalculateType(TypeContext context)
  {
    return context.GetType(symbolName) ?? FactoryType.VoidType;
  }

  public override (string?, string?) PrintSelf()
  {
    return (symbolName, null);
  }

  public new void OverrideType(FactoryType factoryType)
  {
    base.OverrideType(factoryType);
  }

  public override FactoryType? GetHoverType()
  {
    return FactoryType;
  }

  public override string? GetNodeHoverString()
  {
    if (FactoryType is MethodType method)
    {
      return method.ToShortString();
    }
    else if (refInfo != null)
    {
      return $"{refInfo.ToShortString()}: {FactoryType.ToShortString()}";
    }
    else
    {
      if (FactoryLanguage.ResolveGlobal(symbolName) is object o)
      {
        return $"{symbolName}: {o.GetType().Name}";
      }
    }
    return null;
  }

  public override bool Equivalent(object other)
  {
    return other is SymbolNode symNode && this.symbolName == symNode.symbolName;
  }
}
