using static Factory.Superpower.ASTBuilder;
using static Factory.Superpower.SuperpowerParser;

public class ParserTests
{
  // Symbol Tests

  [Fact]
  public void CanParseSymbol()
  {
    var expected = Sym("value");
    var actual = ParseString("value", SymbolParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Sym("refHerring")));

    Assert.Equal((0, 5), actual.Range);
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
      Spread("parent")
    );
    var actual = ParseString(
      "line Test in IronOre out IronIngot alt SolidIronIngot tally inline IronOre limit 60 IronOre ...parent",
      StatementParser
    );

    Assert.True(expected.Equivalent(actual));

    Assert.False(
      actual.Equivalent(
        Line(
          "redHerring",
          In("IronOre"),
          Out("IronIngot"),
          Alt("SolidIronIngot"),
          Tally(true, "IronOre"),
          Limit(LimitVal(60, "IronOre")),
          Spread("parent")
        )
      )
    );

    Assert.False(
      actual.Equivalent(
        Line(
          "Test",
          In("RedHerring"),
          Out("IronIngot"),
          Alt("SolidIronIngot"),
          Tally(true, "IronOre"),
          Limit(LimitVal(60, "IronOre")),
          Spread("parent")
        )
      )
    );

    Assert.False(
      actual.Equivalent(
        Line(
          "Test",
          In("IronOre"),
          Out("IronIngot"),
          Alt("SolidIronIngot"),
          Tally(true, "IronOre"),
          Limit(LimitVal(60, "IronOre"))
        )
      )
    );

    Assert.False(
      actual.Equivalent(
        Line(
          "Test",
          In("IronOre"),
          Out("IronIngot"),
          Alt("SolidIronIngot"),
          Tally(true, "IronOre"),
          Limit(LimitVal(60, "IronOre")),
          Spread("parent"),
          Spread("parent")
        )
      )
    );

    Assert.Equal((0, 101), actual.Range);
  }

  // In Expression

  [Fact]
  public void CanParseInExpression()
  {
    var expected = In("IronOre");
    var actual = ParseString("in IronOre", LineValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(In("CopperOre")));
    Assert.False(actual.Equivalent(Out("IronOre")));

    Assert.Equal((0, 10), actual.Range);
  }

  // Out Expression

  [Fact]
  public void CanParseOutExpression()
  {
    var expected = Out("IronOre");
    var actual = ParseString("out IronOre", LineValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Out("CopperOre")));
    Assert.False(actual.Equivalent(In("IronOre")));

    Assert.Equal((0, 11), actual.Range);
  }

  // Alt Expression

  [Fact]
  public void CanParseAltExpression()
  {
    var expected = Alt("IronOre");
    var actual = ParseString("alt IronOre", LineValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Alt("CoperOre")));
    Assert.False(actual.Equivalent(In("IronOre")));

    Assert.Equal((0, 11), actual.Range);
  }

  // Tally Expression

  [Fact]
  public void CanParseTallyExpression()
  {
    var expected = Tally(false, "IronOre");
    var actual = ParseString("tally IronOre", LineValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Tally(true, "IronOre")));
    Assert.False(actual.Equivalent(Tally(false, "CopperOre")));

    Assert.Equal((0, 13), actual.Range);
  }

  [Fact]
  public void CanParseTallyInlineExpression()
  {
    var expected = Tally(true, "IronOre");
    var actual = ParseString("tally inline IronOre", LineValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Tally(false, "IronOre")));
    Assert.False(actual.Equivalent(Tally(true, "CopperOre")));

    Assert.Equal((0, 20), actual.Range);
  }

  // Limit Expressions

  [Fact]
  public void CanParseLimitExpression()
  {
    var expected = Limit(LimitVal(60, "IronOre"));
    var actual = ParseString("limit 60 IronOre", LineValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Limit(LimitVal(30, "IronOre"))));
    Assert.False(actual.Equivalent(Limit(LimitVal(60, "CopperOre"))));

    Assert.Equal((0, 16), actual.Range);
  }

  [Fact]
  public void CanParseMultiLimitExpression()
  {
    var expected = Limit(LimitVal(60, "IronOre"), LimitVal(30, "CopperOre"));
    var actual = ParseString("limit 60 IronOre 30 CopperOre", LineValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Limit(LimitVal(30, "IronOre"), LimitVal(30, "CopperOre"))));
    Assert.False(actual.Equivalent(Limit(LimitVal(60, "CopperOre"), LimitVal(30, "CopperOre"))));
    Assert.False(actual.Equivalent(Limit(LimitVal(60, "IronOre"))));

    Assert.Equal((0, 29), actual.Range);
  }

  // Print Expression
  [Fact]
  public void CanParsePrint()
  {
    var expected = Print(Chain("value"));
    var actual = ParseString("print value", StatementParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Print(Chain("redHerring"))));

    Assert.Equal((0, 11), actual.Range);
  }

  // Assign Expression

  [Fact]
  public void CanParseAssign()
  {
    var expected = Assign(Chain("left"), Chain("right"));
    var actual = ParseString("let left = right", StatementParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Assign(Chain("right"), Chain("right"))));
    Assert.False(actual.Equivalent(Assign(Chain("left"), Chain("left"))));

    Assert.Equal((0, 16), actual.Range);
  }

  // Term Expression

  [Fact]
  public void CanParseAdd()
  {
    var expected = MathExp(NumLit("1"), TermChain("+", NumLit("2")));
    var actual = ParseString("1 + 2", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(MathExp(NumLit("2"), TermChain("+", NumLit("2")))));
    Assert.False(actual.Equivalent(MathExp(NumLit("1"), TermChain("-", NumLit("2")))));

    Assert.Equal((0, 5), actual.Range);
  }

  [Fact]
  public void CanParseSub()
  {
    var expected = MathExp(NumLit("1"), TermChain("-", NumLit("2")));
    var actual = ParseString("1 - 2", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(MathExp(NumLit("2"), TermChain("-", NumLit("2")))));
    Assert.False(actual.Equivalent(MathExp(NumLit("1"), TermChain("+", NumLit("2")))));

    Assert.Equal((0, 5), actual.Range);
  }

  [Fact]
  public void CanParseTermSequence()
  {
    var expected = MathExp(NumLit("1"), TermChain("+", NumLit("2")), TermChain("-", NumLit("3")));
    var actual = ParseString("1 + 2 - 3", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(
      actual.Equivalent(
        MathExp(NumLit("2"), TermChain("+", NumLit("2")), TermChain("-", NumLit("3")))
      )
    );
    Assert.False(actual.Equivalent(MathExp(NumLit("1"), TermChain("+", NumLit("2")))));

    Assert.Equal((0, 9), actual.Range);
  }

  [Fact]
  public void CanParseTermsWithFactors()
  {
    var expected = MathExp(
      Term(NumLit("1"), FactorChain("*", NumLit("2"))),
      TermChain("+", Term(NumLit("3"), FactorChain("*", NumLit("4"))))
    );

    var actual = ParseString("1 * 2 + 3 * 4", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(
      actual.Equivalent(
        MathExp(
          Term(NumLit("2"), FactorChain("*", NumLit("2"))),
          TermChain("+", Term(NumLit("3"), FactorChain("*", NumLit("4"))))
        )
      )
    );
    Assert.False(actual.Equivalent(MathExp(Term(NumLit("1"), FactorChain("*", NumLit("2"))))));

    Assert.Equal((0, 13), actual.Range);
  }

  [Fact]
  public void CanParseTermsWithParenthetical()
  {
    var expected = Term(
      NumLit("1"),
      FactorChain("*", Paren(MathExp(NumLit("2"), TermChain("+", NumLit("3")))))
    );
    var actual = ParseString("1 * (2 + 3)", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(
      actual.Equivalent(
        Term(
          NumLit("2"),
          FactorChain("*", Paren(MathExp(NumLit("2"), TermChain("+", NumLit("3")))))
        )
      )
    );
    Assert.False(actual.Equivalent(Term(NumLit("1"))));

    Assert.Equivalent((0, 11), actual.Range);
  }

  // Factor Expressions

  [Fact]
  public void CanParseMultiply()
  {
    var expected = Term(NumLit("1"), FactorChain("*", NumLit("2")));
    var actual = ParseString("1 * 2", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Term(NumLit("2"), FactorChain("*", NumLit("2")))));
    Assert.False(actual.Equivalent(Term(NumLit("1"), FactorChain("/", NumLit("2")))));

    Assert.Equal((0, 5), actual.Range);
  }

  [Fact]
  public void CanParseDivide()
  {
    var expected = Term(NumLit("1"), FactorChain("/", NumLit("2")));
    var actual = ParseString("1 / 2", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Term(NumLit("2"), FactorChain("/", NumLit("2")))));
    Assert.False(actual.Equivalent(Term(NumLit("1"), FactorChain("*", NumLit("2")))));

    Assert.Equal((0, 5), actual.Range);
  }

  [Fact]
  public void CanParseModulo()
  {
    var expected = Term(NumLit("1"), FactorChain("%", NumLit("2")));
    var actual = ParseString("1 % 2", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Term(NumLit("2"), FactorChain("%", NumLit("2")))));
    Assert.False(actual.Equivalent(Term(NumLit("1"), FactorChain("*", NumLit("2")))));

    Assert.Equal((0, 5), actual.Range);
  }

  [Fact]
  public void CanParseFactorSequence()
  {
    var expected = Term(
      NumLit("1"),
      FactorChain("*", NumLit("2")),
      FactorChain("/", NumLit("3")),
      FactorChain("%", NumLit("4"))
    );

    var actual = ParseString("1 * 2 / 3 % 4", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(
      actual.Equivalent(
        Term(NumLit("1"), FactorChain("*", NumLit("2")), FactorChain("/", NumLit("3")))
      )
    );

    Assert.Equal((0, 13), actual.Range);
  }

  // Math Unit
  [Fact]
  public void CanParseParentheticalUnit()
  {
    var expected = Paren(NumLit("1.1"));
    var actual = ParseString("(1.1)", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(NumLit("1.1")));
    Assert.False(actual.Equivalent(Paren(StrLit(""))));
    Assert.False(actual.Equivalent(Paren(NumLit("2"))));

    Assert.Equal((0, 5), actual.Range);
  }

  // Chain Tests

  [Fact]
  public void CanParseSingleLeftHandExp()
  {
    var expected = Chain("value");
    var actual = ParseString("value", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Chain("redHerring")));

    Assert.Equal((0, 5), actual.Range);
  }

  [Fact]
  public void CanParseDerefLHS()
  {
    var expected = Chain("value", Deref("field"));
    var actual = ParseString("value.field", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Chain("value")));
    Assert.False(actual.Equivalent(Chain("value", Deref("refHerring"))));

    Assert.Equal((0, 11), actual.Range);
  }

  [Fact]
  public void CanParseMultiDeref()
  {
    var expected = Chain("value", Deref("a"), Deref("b"));
    var actual = ParseString("value.a.b", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Chain("value")));
    Assert.False(actual.Equivalent(Chain("value", Deref("a"))));
    Assert.False(actual.Equivalent(Chain("value", Deref("a"), Deref("a"))));
    Assert.False(actual.Equivalent(Chain("value", Deref("a"), Deref("b"), Deref("c"))));

    Assert.Equal((0, 9), actual.Range);
  }

  // Invocation Tests

  [Fact]
  public void CanParseInvocationLHS()
  {
    var expected = Chain("value", Invoke());
    var actual = ParseString("value()", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Chain("value")));
    Assert.False(actual.Equivalent(Chain("redHerring", Invoke())));
    Assert.False(actual.Equivalent(Chain("value", Invoke(Chain("argument")))));

    Assert.Equal((0, 7), actual.Range);
  }

  [Fact]
  public void CanParseInvocationWithArgs()
  {
    var expected = Chain("value", Invoke(Chain("argument")));
    var actual = ParseString("value(argument)", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Chain("value", Invoke(Chain("redHerring")))));
    Assert.False(actual.Equivalent(Chain("redHerring", Invoke(Chain("argument")))));
    Assert.False(
      actual.Equivalent(Chain("redHerring", Invoke(Chain("argument"), Chain("argument"))))
    );
    Assert.False(actual.Equivalent(Chain("value", Invoke())));

    Assert.Equal((0, 15), actual.Range);
  }

  [Fact]
  public void CanParseMultiInvocation()
  {
    var expected = Chain("value", Invoke(), Invoke());
    var actual = ParseString("value()()", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(Chain("value", Invoke())));
    Assert.False(actual.Equivalent(Chain("redHerring", Invoke(), Invoke())));

    Assert.Equal((0, 9), actual.Range);
  }

  // Literal Tests

  [Fact]
  public void CanParseStringExp()
  {
    var expected = StrLit("string");
    var actual = ParseString("\"string\"", ValueExpParser);

    Assert.True(expected.Equivalent(actual));

    Assert.False(actual.Equivalent(StrLit("redHerring")));

    Assert.Equal((0, 8), actual.Range);
  }

  [Fact]
  public void CanParseNumberExp()
  {
    var expected = NumLit("1.1");
    var actual = ParseString("1.1", ValueExpParser);
    Assert.True(expected.Equivalent(actual));

    // Red Herrings
    Assert.False(actual.Equivalent(NumLit("1")));

    // Span
    Assert.Equal((0, 3), actual.Range);
  }

  // Spread

  [Fact]
  public void CanParseSpread()
  {
    var expected = Spread("value");
    var actual = ParseString("...value", SpreadParser);
    Assert.True(expected.Equivalent(actual));

    // Red Herrings
    Assert.False(actual.Equivalent(Sym("value")));
    Assert.False(actual.Equivalent(Spread("redHerring")));

    // Span
    Assert.Equal((0, 8), actual.Range);
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
