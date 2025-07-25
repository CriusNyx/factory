using SpracheParser;

public class AssignExpression(
  LeftHandExpression leftHandExpression,
  RightHandExpression rightHandExpression
) : Statement
{
  public LeftHandExpression Left => leftHandExpression;
  public RightHandExpression Right => rightHandExpression;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is AssignExpression assign
      && Left.Equivalent(assign.Left)
      && Right.Equivalent(assign.Right);
  }
}
