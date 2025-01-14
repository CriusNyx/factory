using GenParse.Functional;
using static GenParse.Util.Formatting;

namespace Factory;

public abstract class LanguageNode : ITree<LanguageNode>
{
  private FactoryType factoryType;

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
}
