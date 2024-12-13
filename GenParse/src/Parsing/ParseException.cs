using GenParse.Lexing;
using GenParse.Parsing;

public class ParseException<LexonType> : Exception
{
  public readonly Lexon<LexonType> lexon;

  public ParseException(Lexon<LexonType> lexon)
    : base(GenerateMessage(lexon))
  {
    this.lexon = lexon;
  }

  private static string GenerateMessage(Lexon<LexonType> lexon)
  {
    return $"Unexpected symbol {lexon.sourceCode}";
  }
}
