using SharpParse.Functional;
using SharpParse.Parsing;
using static SharpParse.Util.Formatting;

namespace Factory;

public abstract class LanguageNode : ITree<LanguageNode>
{
  [AST]
  public ASTNode<FactoryLexon> astNode;

  private FactoryType factoryType;

  public FactoryType FactoryType
  {
    get
    {
      if (factoryType == null)
      {
        return FactoryType.ErrorType;
      }
      return factoryType;
    }
  }

  public bool HasIndex(int index)
  {
    if (astNode == null)
    {
      throw new InvalidOperationException("astNode is null!?!?! WTF?");
    }
    var (pos, len) = astNode.CalculatePosition();
    return index >= pos && index < pos + len;
  }

  /// <summary>
  /// If this node contains this index, it will return the type at the index.
  /// Otherwise it will return null
  /// </summary>
  /// <param name="index"></param>
  /// <returns></returns>
  public FactoryType? GetTypeAtIndex(int index)
  {
    if (!HasIndex(index))
    {
      return null!;
    }
    foreach (var child in GetChildren())
    {
      var node = child.To<LanguageNode>();
      if (node.GetTypeAtIndex(index) is FactoryType childType && !childType.IsEmpty())
      {
        return childType;
      }
    }

    return GetHoverType();
  }

  public FactoryType? GetReferenceTypeAtIndex(int index)
  {
    if (!HasIndex(index))
    {
      return null;
    }
    foreach (var child in GetChildren())
    {
      var node = child.To<LanguageNode>();
      var result = node.GetReferenceTypeAtIndex(index);
      if (result != null)
      {
        return result;
      }
    }
    if (this is ExpChainNode expChainNode)
    {
      return expChainNode.refType;
    }
    if (this is ChainNode chainNode)
    {
      return chainNode.refType;
    }
    return null;
  }

  public string? GetHoverString(int index)
  {
    if (!HasIndex(index))
    {
      return null!;
    }
    foreach (var child in GetChildren())
    {
      var node = child.To<LanguageNode>();
      var result = node.GetHoverString(index);
      if (result != null)
      {
        return result;
      }
    }
    return GetNodeHoverString();
  }

  public virtual string? GetNodeHoverString()
  {
    return null;
  }

  public virtual FactoryType? GetHoverType()
  {
    return null!;
  }

  public FactoryType GetFactoryType(TypeContext typeContext)
  {
    factoryType = factoryType ?? CalculateType(typeContext);
    return factoryType;
  }

  public abstract FactoryType CalculateType(TypeContext context);
  public abstract IEnumerable<ITree<LanguageNode>> GetChildren();

  public virtual (string?, string?) PrintSelf()
  {
    return (null, null);
  }

  public string PrintPretty(Func<LanguageNode, string[]> annotate = null!)
  {
    annotate = annotate ?? ((node) => []);
    List<string[]> lines = new List<string[]>();
    PrintPretty(this, "", lines, annotate);
    return PrintGrid(lines.ToArray(), "  ");
  }

  private void PrintPretty(
    LanguageNode node,
    string indent,
    List<string[]> lines,
    Func<LanguageNode, string[]> annotate
  )
  {
    var (start, end) = node.PrintSelf();
    var annotations = annotate(node);

    lines.Add([indent + node.GetType().Name, start ?? "", .. annotations]);

    foreach (var child in node.GetChildren())
    {
      PrintPretty(child.To<LanguageNode>(), indent + "  ", lines, annotate);
    }

    if (end != null)
    {
      lines.Add(["", end, .. annotations.Map(x => "")]);
    }
  }

  protected void OverrideType(FactoryType overrideType)
  {
    if (factoryType != null && !factoryType.IsEmpty())
    {
      Console.WriteLine(
        $"factoryType {factoryType.ToShortString()} is not empty on {GetType().Name} but it is being overridden"
      );
    }
    factoryType = overrideType;
  }
}
