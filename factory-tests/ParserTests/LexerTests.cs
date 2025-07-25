using FactorySpracheParser;
using Sprache;
using static ParserTestUtil;

namespace factory_tests;

public class LexerTests
{
  [Fact]
  public void CanParseComment()
  {
    TokenParserTest(
      FactoryParser.commentParser,
      "// This is a comment",
      SuperpowerTokenType.comment
    );
  }

  [Fact]
  public void CanParseWhitespace()
  {
    TokenParserTest(FactoryParser.whitespaceParser, "  ", SuperpowerTokenType.whitespace);
  }

  [Fact]
  public void CanParseSpread()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "...", SuperpowerTokenType.spread);
  }

  [Fact]
  public void CanPaseSpreadWithComment()
  {
    var expected = SpracheToken.CreateTestToken(SuperpowerTokenType.spread, "...");
    var actual = FactoryParser.anyTokenParser.Parse("... // Comment");

    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanPaseSpreadWithWhitespace()
  {
    var expected = SpracheToken.CreateTestToken(SuperpowerTokenType.spread, "...");
    var actual = FactoryParser.anyTokenParser.Parse("  ...  ");

    Assert.True(expected.Equivalent(actual));
  }

  [Fact]
  public void CanParseDot()
  {
    TokenParserTest(FactoryParser.anyTokenParser, ".", SuperpowerTokenType.dot);
  }

  [Fact]
  public void CanParseComma()
  {
    TokenParserTest(FactoryParser.anyTokenParser, ",", SuperpowerTokenType.comma);
  }

  [Fact]
  public void CanParseOpenParen()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "(", SuperpowerTokenType.openParen);
  }

  [Fact]
  public void CanParseClosedParen()
  {
    TokenParserTest(FactoryParser.anyTokenParser, ")", SuperpowerTokenType.closedParen);
  }

  [Fact]
  public void CanParsePlus()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "+", SuperpowerTokenType.plus);
  }

  [Fact]
  public void CanParseMinus()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "-", SuperpowerTokenType.minus);
  }

  [Fact]
  public void CanParseAsterisk()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "*", SuperpowerTokenType.asterisk);
  }

  [Fact]
  public void CanParseForwardSlash()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "/", SuperpowerTokenType.forwardSlash);
  }

  [Fact]
  public void CanParsePercent()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "%", SuperpowerTokenType.percent);
  }

  [Fact]
  public void CanParseLine()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "line", SuperpowerTokenType.lineKeyword);
  }

  [Fact]
  public void CanParseAlt()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "alt", SuperpowerTokenType.altKeyword);
  }

  [Fact]
  public void CanParseOut()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "out", SuperpowerTokenType.outKeyword);
  }

  [Fact]
  public void CanParsePrint()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "print", SuperpowerTokenType.printKeyword);
  }

  [Fact]
  public void CanParseTally()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "tally", SuperpowerTokenType.tallyKeyword);
  }

  [Fact]
  public void CanParseInline()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "inline", SuperpowerTokenType.inlineKeyword);
  }

  [Fact]
  public void CanParseIn()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "in", SuperpowerTokenType.inKeyword);
  }

  [Fact]
  public void CanParseLimit()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "limit", SuperpowerTokenType.limitKeyword);
  }

  [Fact]
  public void CanPaseString()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "\"\"", SuperpowerTokenType.stringLiteral);
  }

  [Fact]
  public void CanParseStringWithKeyword()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "\"alt\"", SuperpowerTokenType.stringLiteral);
  }

  [Fact]
  public void CanParseNumber()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "1", SuperpowerTokenType.numberLiteral);
  }

  [Fact]
  public void CanParseDecimal()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "1.1", SuperpowerTokenType.numberLiteral);
  }

  [Fact]
  public void CanParseSymbol()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "variable", SuperpowerTokenType.symbol);
  }

  [Fact]
  public void CanParseSymbolWithNum()
  {
    TokenParserTest(FactoryParser.anyTokenParser, "variable1", SuperpowerTokenType.symbol);
  }
}
