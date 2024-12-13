using System.Reflection.Metadata.Ecma335;
using GenParse.Lexing;

namespace GenParse.Parsing;

public abstract class ParseResult<LexonType> { }

public class SuccessParseResult<LexonType>(ASTNode<LexonType> astNode, int lexonsConsumed)
  : ParseResult<LexonType>
{
  public readonly ASTNode<LexonType> astNode = astNode;
  public readonly int lexonsConsumed = lexonsConsumed;
}

public class FailedParseResult<LexonType>(
  Lexon<LexonType> offendingLexon,
  LexonType[] expectedLexons
) : ParseResult<LexonType>
{
  public readonly Lexon<LexonType> offendingLexon = offendingLexon;
  public readonly LexonType[] expectedLexons = expectedLexons;
}

public class AggregateFailedResult<LexonType>(FailedParseResult<LexonType>[] results)
  : ParseResult<LexonType>
{
  FailedParseResult<LexonType>[] results;
};
