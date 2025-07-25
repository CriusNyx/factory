using FactorySpracheParser;
using Sprache;
using static ParserTestUtil;

namespace factory_tests;

public class LexerTests
{
  [Fact]
  public void CanParseComment()
  {
    TokenParserTest(FactoryParser.commentParser, "// This is a comment", SpracheTokenType.comment);
  }

  [Fact]
  public void CanParseWhitespace()
  {
    TokenParserTest(FactoryParser.whitespaceParser, "  ", SpracheTokenType.whitespace);
  }

  [Fact]
  public void CanParseSpread()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "...", SpracheTokenType.spread);
  }

  [Fact]
  public void CanPaseSpreadWithComment()
  {
    var expected = SpracheToken.CreateTestToken(SpracheTokenType.spread, "...");
    var actual = FactoryParser.anyTokenParser.Parse("... // Comment");

    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanPaseSpreadWithWhitespace()
  {
    var expected = SpracheToken.CreateTestToken(SpracheTokenType.spread, "...");
    var actual = FactoryParser.anyTokenParser.Parse("  ...  ");

    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseDot()
  {
    TokenParserTest(FactoryParser.anyTokenParser, ".", SpracheTokenType.dot);
  }

  [Fact]
  public void CanParseComma()
  {
    TokenParserTest(FactoryParser.anyTokenParser, ",", SpracheTokenType.comma);
  }

  [Fact]
  public void CanParseOpenParen()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "(", SpracheTokenType.openParen);
  }

  [Fact]
  public void CanParseClosedParen()
  {
    TokenParserTest(FactoryParser.anyTokenParser, ")", SpracheTokenType.closedParen);
  }

  [Fact]
  public void CanParsePlus()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "+", SpracheTokenType.plus);
  }

  [Fact]
  public void CanParseMinus()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "-", SpracheTokenType.minus);
  }

  [Fact]
  public void CanParseAsterisk()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "*", SpracheTokenType.asterisk);
  }

  [Fact]
  public void CanParseForwardSlash()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "/", SpracheTokenType.forwardSlash);
  }

  [Fact]
  public void CanParsePercent()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "%", SpracheTokenType.percent);
  }

  [Fact]
  public void CanParseLine()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "line", SpracheTokenType.lineKeyword);
  }

  [Fact]
  public void CanParseAlt()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "alt", SpracheTokenType.altKeyword);
  }

  [Fact]
  public void CanParseOut()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "out", SpracheTokenType.outKeyword);
  }

  [Fact]
  public void CanParsePrint()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "print", SpracheTokenType.printKeyword);
  }

  [Fact]
  public void CanParseTally()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "tally", SpracheTokenType.tallyKeyword);
  }

  [Fact]
  public void CanParseInline()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "inline", SpracheTokenType.inlineKeyword);
  }

  [Fact]
  public void CanParseIn()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "in", SpracheTokenType.inKeyword);
  }

  [Fact]
  public void CanParseLimit()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "limit", SpracheTokenType.limitKeyword);
  }

  [Fact]
  public void CanPaseString()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "\"\"", SpracheTokenType.stringLiteral);
  }

  [Fact]
  public void CanParseStringWithKeyword()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "\"alt\"", SpracheTokenType.stringLiteral);
  }

  [Fact]
  public void CanParseNumber()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "1", SpracheTokenType.numberLiteral);
  }

  [Fact]
  public void CanParseDecimal()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "1.1", SpracheTokenType.numberLiteral);
  }

  [Fact]
  public void CanParseSymbol()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "variable", SpracheTokenType.symbol);
  }

  [Fact]
  public void CanParseSymbolWithNum()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "variable1", SpracheTokenType.symbol);
  }
}
