using SharpParse.Util;

namespace Factory;

public class LiteralExpNode : LanguageNode
{
  public override FactoryType CalculateType(TypeContext context)
  {
    throw new NotImplementedException();
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { };
  }
}
