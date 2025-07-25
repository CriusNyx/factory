using SpracheParser;

public enum BinaryOperation
{
  add,
  sub,
  mul,
  div,
  mod,
}

public enum UnaryOperation
{
  realNegate,
}

public class BinaryExpression(
  BinaryOperation operation,
  RightHandExpression lhs,
  RightHandExpression rhs
) : RightHandExpression
{
  public BinaryOperation Operation => operation;
  public RightHandExpression LHS => lhs;
  public RightHandExpression RHS => rhs;

  public static BinaryOperation ParseOp(string source)
  {
    switch (source)
    {
      case "+":
        return BinaryOperation.add;
      case "-":
        return BinaryOperation.sub;
      case "*":
        return BinaryOperation.mul;
      case "/":
        return BinaryOperation.div;
      case "%":
        return BinaryOperation.mod;
      default:
        throw new InvalidOperationException();
    }
  }

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is BinaryExpression binExp
      && Operation == binExp.Operation
      && LHS.Equivalent(binExp.LHS)
      && RHS.Equivalent(binExp.RHS);
  }
}

public class UnaryExpression(UnaryOperation operation, RightHandExpression value)
  : RightHandExpression
{
  public UnaryOperation Operation => operation;
  public RightHandExpression Value => value;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is UnaryExpression unExp
      && Operation == unExp.Operation
      && Value.Equivalent(unExp.Value);
  }
}
