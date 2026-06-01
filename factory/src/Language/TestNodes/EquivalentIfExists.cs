using Factory;
using Factory.Util;

public class EquivalentIfExists : LanguageNode
{
  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    throw new NotImplementedException();
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    throw new NotImplementedException();
  }

  public override bool Equivalent(object other)
  {
    return other != null;
  }
}
