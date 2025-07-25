using SpracheParser;

public class ParentheticalUnit(RightHandExpression value) : ASTNode, RightHandExpression
{
  public RightHandExpression Value => value;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is ParentheticalUnit parenUnit && Value.Equivalent(parenUnit.Value);
  }
}
