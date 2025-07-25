namespace Factory;

public abstract class LineExpNode : ValueNode
{
  public LineExpNode() { }

  public LineExpNode(SourceCodeInfo sourceInfo)
    : base(sourceInfo) { }
}
