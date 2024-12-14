using Factory.Parsing;
using GenParse.Lexing;
using GenParse.Parsing;

public class FactoryParseException(
  string sourceLocation,
  string sourceCode,
  FailedParseResult<FactoryLexon> failed
) : ParseException<FactoryLexon>(failed)
{
  public readonly string sourceLocation = sourceLocation;
  public readonly string sourceCode = sourceCode;
}
