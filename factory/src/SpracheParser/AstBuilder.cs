// Used for building AST's for tests
using FactorySpracheParser;
using SharpParse.Functional;
using SpracheParser;

public static class AstBuilder
{
  public static ParentheticalUnit Paren(RightHandExpression value)
  {
    return new ParentheticalUnit(value);
  }

  public static AssignExpression Assign(LeftHandExpression lhs, RightHandExpression rhs)
  {
    return new AssignExpression(lhs, rhs);
  }

  public static Symbol Sym(string value)
  {
    return new Symbol(SpracheToken.CreateTestToken(SuperpowerTokenType.symbol, value));
  }

  public static NumberLiteral NumLit(string value)
  {
    return new NumberLiteral(
      SpracheToken.CreateTestToken(SuperpowerTokenType.numberLiteral, value)
    );
  }

  public static StringLiteral StrLit(string value)
  {
    return new StringLiteral(
      SpracheToken.CreateTestToken(SuperpowerTokenType.stringLiteral, value)
    );
  }

  public static BinaryExpression BinExp(string op, RightHandExpression lhs, RightHandExpression rhs)
  {
    return new BinaryExpression(BinaryExpression.ParseOp(op), lhs, rhs);
  }

  public static LeftHandExpression LHS(
    string symbol,
    params Func<LHSContinue?, LHSContinue>[] chain
  )
  {
    return new LeftHandExpression(
      Sym(symbol),
      chain.Reverse().Aggregate(null as LHSContinue, (acc, func) => func(acc))
    );
  }

  public static Func<LHSContinue?, LHSContinue> Deref(string symbol)
  {
    return (next) => new LHSContinue(new Deref(Sym(symbol)), next);
  }

  public static Func<LHSContinue?, LHSContinue> Invoke(params ASTNode[] arguments)
  {
    return (next) => new LHSContinue(new Invocation(arguments), next);
  }

  public static LineSpreadStatement RecSpread(RightHandExpression value)
  {
    return new LineSpreadStatement(new SpreadExpression(value));
  }

  public static SpreadExpression Spread(RightHandExpression value)
  {
    return new SpreadExpression(value);
  }

  public static LineExpression RecExp(LineExpressionType type, params LineValueExpression[] values)
  {
    return new LineExpression(type, values);
  }

  public static LineExpression RecExp(LineExpressionType type, params string[] values) =>
    new LineExpression(type, values.Map((x) => RecVal(type, x)));

  public static LineValueExpression RecVal(LineExpressionType type, string sym)
  {
    return new LineValueExpression(type, Sym(sym));
  }

  public static ProductionLine Line(string name, params LineStatement[] expressions)
  {
    return new ProductionLine(Sym(name), expressions);
  }

  public static LineExpression Out(params string[] syms) => RecExp(LineExpressionType.@out, syms);

  public static LineExpression In(params string[] syms) => RecExp(LineExpressionType.@in, syms);

  public static LineExpression Alt(params string[] syms) => RecExp(LineExpressionType.alt, syms);

  public static LineExpression Tally(bool inline, params string[] syms)
  {
    return new LineExpression(
      LineExpressionType.tally,
      syms.Map(x => RecVal(LineExpressionType.tally, x)),
      inline
    );
  }

  public static LineExpression Limit(params LineValueExpression[] values)
  {
    return new LineExpression(LineExpressionType.limit, values);
  }

  public static LineValueExpression LimitVal(RightHandExpression scalar, string sym)
  {
    return new LineValueExpression(LineExpressionType.limit, Sym(sym), scalar);
  }

  public static LineValueExpression LimitVal(decimal numVal, string sym)
  {
    return new LineValueExpression(LineExpressionType.limit, Sym(sym), NumLit(numVal.ToString()));
  }
}
