using SharpParse.Parsing;

namespace Factory;

public class FactoryParseException(
  string sourceLocation,
  string sourceCode,
  FailedParseResult<FactoryLexon> failed
) : ParseException<FactoryLexon>(failed)
{
  public readonly string sourceLocation = sourceLocation;
  public readonly string sourceCode = sourceCode;
}
