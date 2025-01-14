using static GenParse.Util.Formatting;

namespace Factory;

public abstract class LanguageNode : ITree<LanguageNode>
{
  public abstract FactoryType CalculateType(TypeContext context);
  public abstract IEnumerable<ITree<LanguageNode>> GetChildren();
}
