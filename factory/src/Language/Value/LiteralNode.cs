namespace Factory;

public abstract class LiteralNode : ValueNode
{
  public LiteralNode() { }

  public LiteralNode(SourceCodeInfo sourceInfo)
    : base(sourceInfo) { }

  public override FactoryType? GetHoverType()
  {
    return FactoryType;
  }
}
