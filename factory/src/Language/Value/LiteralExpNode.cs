using GenParse.Util;

namespace Factory;

public class LiteralExpNode : LanguageNode
{
  public FactoryType CalculateType(TypeContext context)
  {
    throw new NotImplementedException();
  }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { };
  }
}
