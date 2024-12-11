using GenParse.Lexing;
using GenParse.Parsing;

public interface CustomParser<LexonType>
{
  public ParseResult<LexonType>? Parse(
    ParseContext<LexonType> context,
    Lexon<LexonType>[] lexons,
    int index
  );
}
