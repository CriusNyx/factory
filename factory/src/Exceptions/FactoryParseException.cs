using SharpParse.Parsing;

namespace Factory;

public class FactoryParseException(
  string sourceLocation,
  string sourceCode,
  FailedParseResult failed
) : ParseException(failed)
{
  public readonly string sourceLocation = sourceLocation;
  public readonly string sourceCode = sourceCode;
}
