using FactorySpracheParser;
using SpracheParser;

public class NumberLiteral(SpracheToken source) : ProgramEquivalent, RightHandExpression
{
  public SpracheToken Source => source;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is NumberLiteral numberLiteral && Source.Equivalent(numberLiteral.Source);
  }
}

public class StringLiteral(SpracheToken source) : ProgramEquivalent, RightHandExpression
{
  public SpracheToken Source => source;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is StringLiteral stringLiteral && source.Equivalent(stringLiteral.Source);
  }
}
