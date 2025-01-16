namespace Factory;

public abstract class LiteralNode : ValueNode
{
  public override FactoryType? GetHoverType()
  {
    return this.FactoryType;
  }
}
