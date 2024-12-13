using Factory.Parsing;

namespace Factory;

public class FactorySemanticToken(
  int position,
  int length,
  FactorySemanticType semanticType,
  int modifier
)
{
  public int position { get; private set; } = position;
  public int length { get; private set; } = length;
  public FactorySemanticType semanticType { get; private set; } = semanticType;
  public int modifier { get; private set; } = modifier;

  public override string ToString()
  {
    return semanticType.ToString();
  }
}
