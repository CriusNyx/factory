using static AstBuilder;
using static SuperpowerParser.FactoryParser;

public class ParserTests
{
  // Symbol Tests

  [Fact]
  public void CanParseSymbol()
  {
    var expected = Sym("value");
    var actual = ParseString("value", SymbolParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseProductionLine()
  {
    var expected = Line(
      "Test",
      In("IronOre"),
      Out("IronIngot"),
      Alt("SolidIronIngot"),
      Tally(true, "IronOre"),
      Limit(LimitVal(60, "IronOre")),
      RecSpread(LHS("parent"))
    );
    var actual = ParseString(
      "line Test in IronOre out IronIngot alt SolidIronIngot tally inline IronOre limit 60 IronOre ...parent;",
      StatementParser
    );
    Assert.True(expected.Equivalent(actual));
  }

  // Out Expression

  [Fact]
  public void CanParseOutExpression()
  {
    var expected = Out("IronOre");
    var actual = ParseString("out IronOre", LineStatementParser);
    Assert.True(expected.Equivalent(actual));
  }

  // In Expression

  [Fact]
  public void CanParseInExpression()
  {
    var expected = In("IronOre");
    var actual = ParseString("in IronOre", LineStatementParser);
    Assert.True(expected.Equivalent(actual));
  }

  // Alt Expression

  [Fact]
  public void CanParseAltExpression()
  {
    var expected = Alt("IronOre");
    var actual = ParseString("alt IronOre", LineStatementParser);
    Assert.True(expected.Equivalent(actual));
  }

  // Tally Expression

  [Fact]
  public void CanParseTallyExpression()
  {
    var expected = Tally(false, "IronOre");
    var actual = ParseString("tally IronOre", LineStatementParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseTallyInlineExpression()
  {
    var expected = Tally(true, "IronOre");
    var actual = ParseString("tally inline IronOre", LineStatementParser);
    Assert.True(expected.Equivalent(actual));
  }

  // Limit Expressions

  [Fact]
  public void CanParseLimitExpression()
  {
    var expected = Limit(LimitVal(60, "IronOre"));
    var actual = ParseString("limit 60 IronOre", LineStatementParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseMultiLimitExpression()
  {
    var expected = Limit(LimitVal(60, "IronOre"), LimitVal(30, "CopperOre"));
    var actual = ParseString("limit 60 IronOre 30 CopperOre", LineStatementParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseRecipeSpread()
  {
    var expected = RecSpread(LHS("value"));
    var actual = ParseString("...value", LineStatementParser);
    Assert.True(expected.Equivalent(actual));
  }

  // Assign Expression

  [Fact]
  public void CanParseAssign()
  {
    var expected = Assign(LHS("left"), LHS("right"));
    var actual = ParseString("left = right;", StatementParser);
    Assert.True(expected.Equivalent(actual));
  }

  // Term Expression

  [Fact]
  public void CanParseAdd()
  {
    var expected = BinExp("+", NumLit("1"), NumLit("2"));
    var actual = ParseString("1 + 2", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseSub()
  {
    var expected = BinExp("-", NumLit("1"), NumLit("2"));
    var actual = ParseString("1 - 2", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseTermSequence()
  {
    var expected = BinExp("-", BinExp("+", NumLit("1"), NumLit("2")), NumLit("3"));
    var actual = ParseString("1 + 2 - 3", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseTermsWithFactors()
  {
    var expected = BinExp(
      "+",
      BinExp("*", NumLit("1"), NumLit("2")),
      BinExp("*", NumLit("3"), NumLit("4"))
    );

    var actual = ParseString("1 * 2 + 3 * 4", RightHandExpressionParser);

    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseTermsWithParenthetical()
  {
    var expected = BinExp("*", NumLit("1"), Paren(BinExp("+", NumLit("2"), NumLit("3"))));
    var actual = ParseString("1 * (2 + 3)", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  // Factor Expressions

  [Fact]
  public void CanParseMultiply()
  {
    var expected = BinExp("*", NumLit("1"), NumLit("2"));
    var actual = ParseString("1 * 2", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseDivide()
  {
    var expected = BinExp("/", NumLit("1"), NumLit("2"));
    var actual = ParseString("1 / 2", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseModulo()
  {
    var expected = BinExp("%", NumLit("1"), NumLit("2"));
    var actual = ParseString("1 % 2", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseFactorSequence()
  {
    var expected = BinExp(
      "%",
      BinExp("/", BinExp("*", NumLit("1"), NumLit("2")), NumLit("3")),
      NumLit("4")
    );

    var actual = ParseString("1 * 2 / 3 % 4", RightHandExpressionParser);

    Assert.True(expected.Equivalent(actual));
  }

  // Math Unit
  [Fact]
  public void CanParseParentheticalUnit()
  {
    var expected = Paren(NumLit("1.1"));
    var actual = ParseString("(1.1)", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  // Chain Tests

  [Fact]
  public void CanParseSingleLeftHandExp()
  {
    var expected = LHS("value");
    var actual = ParseString("value", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseDerefLHS()
  {
    var expected = LHS("value", Deref("field"));
    var actual = ParseString("value.field", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseMultiDeref()
  {
    var expected = LHS("value", Deref("a"), Deref("b"));
    var actual = ParseString("value.a.b", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  // Invocation Tests

  [Fact]
  public void CanParseInvocationLHS()
  {
    var expected = LHS("value", Invoke());
    var actual = ParseString("value()", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseInvocationWithArgs()
  {
    var expected = LHS("value", Invoke(LHS("argument")));
    var actual = ParseString("value(argument)", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseMultiInvocation()
  {
    var expected = LHS("value", Invoke(), Invoke());
    var actual = ParseString("value()()", RightHandExpressionParser);
    Assert.True(expected.Equivalent(actual));
  }

  // Literal Tests

  [Fact]
  public void CanParseStringExp()
  {
    var expected = StrLit("\"string\"");
    var actual = ParseString("\"string\"", StringLiteralParser);
    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseNumberExp()
  {
    var expected = NumLit("1.1");
    var actual = ParseString("1.1", NumberLiteralParser);
    Assert.True(expected.Equivalent(actual));
  }

  // Spread

  [Fact]
  public void CanParseSpread()
  {
    var expected = Spread(LHS("value"));
    var actual = ParseString("...value", SpreadParser);
    Assert.True(expected.Equivalent(actual));
  }

  // Edge case tests
  [Fact]
  public void CanParseKeywordSoup()
  {
    var expected = Sym("inout");
    var actual = ParseString("inout", SymbolParser);
    Assert.True(expected.Equivalent(actual));
  }
}
