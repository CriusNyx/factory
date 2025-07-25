using Factory.Superpower;

public class LexerTests
{
  [Fact]
  public void CanLexBadFile()
  {
    var results = SuperpowerTokenizer.Tokenize("@ line");
    var actual = results.Select(x => x.Kind);
    SuperpowerTokenType[] expected = [SuperpowerTokenType.unknown, SuperpowerTokenType.lineKeyword];
    Assert.Equal(expected, actual);
  }
}
