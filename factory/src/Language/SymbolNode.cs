using Microsoft.VisualBasic.FileIO;
using SharpParse.Functional;
using SharpParse.Util;

namespace Factory;

[ASTClass("symbol")]
public class SymbolNode : ValueNode
{
  public string symbolName => astNode.SourceCode();
  private RefInfo _refInfo;
  public RefInfo refInfo
  {
    get => _refInfo;
    set { _refInfo = value; }
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { };
  }

  public SymbolVal Evaluate()
  {
    return new SymbolVal(symbolName);
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return Evaluate().With(context);
  }

  public override string ToString() => $"Symbol {astNode.SourceCode()}";

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
}
