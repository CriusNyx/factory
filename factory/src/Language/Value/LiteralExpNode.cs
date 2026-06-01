using Factory.Util;

namespace Factory;

public class LiteralExpNode : LanguageNode
{
  protected override FactoryType CalculateType(TypeContext context)
  {
    throw new NotImplementedException();
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [];
  }
}
