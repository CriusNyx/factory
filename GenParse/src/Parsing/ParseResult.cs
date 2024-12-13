namespace GenParse.Parsing;

public abstract class ParseResult<LexonType> { }

public class SuccessParseResult<LexonType> : ParseResult<LexonType>
{
  public readonly ASTNode<LexonType> astNode;
  public readonly int lexonsConsumed;

  public SuccessParseResult(ASTNode<LexonType> astNode, int lexonsConsumed)
  {
    this.astNode = astNode;
    this.lexonsConsumed = lexonsConsumed;
  }
}

public class FailedParseResult<LexonType> : ParseResult<LexonType> { }
