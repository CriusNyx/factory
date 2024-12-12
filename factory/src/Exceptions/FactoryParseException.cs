using Factory.Parsing;
using GenParse.Lexing;

public class FactoryParseException(
  string sourceLocation,
  string sourceCode,
  Lexon<FactoryLexon> lexon
) : ParseException<FactoryLexon>(lexon)
{
  public readonly string sourceLocation = sourceLocation;
  public readonly string sourceCode = sourceCode;
}
