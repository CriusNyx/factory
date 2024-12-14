using GenParse.Util;

namespace Factory;

public class LiteralExpNode : LanguageNode
{
  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { };
  }
}
