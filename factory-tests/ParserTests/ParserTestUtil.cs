using FactorySpracheParser;
using Sprache;

public static class ParserTestUtil
{
  public static void TokenParserTest(
    Parser<SpracheToken> parser,
    string source,
    SpracheTokenType type
  )
  {
    var expected = SpracheToken.CreateTestToken(type, source);
    var actual = parser.Parse(source);

    Assert.True(expected.Equivalent(actual));
  }
}
