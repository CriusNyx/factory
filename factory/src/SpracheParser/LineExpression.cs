using SharpParse.Functional;
using SpracheParser;

public interface LineStatement : ASTNode { }

public class ProductionLine(Symbol name, LineStatement[] statements) : Statement
{
  public Symbol Name => name;
  public LineStatement[] Statements => statements;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is ProductionLine line
      && Name.Equivalent(line.Name)
      && Statements.Equivalent(line.Statements);
  }
}

public enum LineExpressionType
{
  @out,
  @in,
  alt,
  tally,
  limit,
}

public class LineExpression(
  LineExpressionType type,
  LineValueExpression[] values,
  bool inline = false
) : LineStatement
{
  public LineExpressionType Type => type;
  public bool Inline => inline;
  public LineValueExpression[] Values => values;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is LineExpression exp
      && Type == exp.Type
      && Values
        .OuterZip(exp.Values)
        .All(
          (pair) =>
          {
            var (a, b) = pair;
            return a.SafeEquivalent(b);
          }
        );
  }
}

public class LineValueExpression(
  LineExpressionType type,
  Symbol symbol,
  RightHandExpression? scalarValue = null
) : ASTNode
{
  public LineExpressionType Type => type;
  public Symbol Symbol => symbol;
  public RightHandExpression? ScalarValue => scalarValue;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is LineValueExpression val
      && Type == val.Type
      && ScalarValue.SafeEquivalent(val.ScalarValue)
      && Symbol.Equivalent(val.Symbol);
  }
}

public class LineSpreadStatement(SpreadExpression spread) : ASTNode, LineStatement
{
  public SpreadExpression Spread => spread;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is LineSpreadStatement spread && Spread.Equivalent(spread.Spread);
  }
}
