using SpracheParser;

public class SpreadExpression(RightHandExpression value) : ProgramEquivalent, ASTNode
{
  public RightHandExpression Value => value;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is SpreadExpression valExp && Value.Equivalent(valExp.Value);
  }
}
