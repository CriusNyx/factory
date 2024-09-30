namespace GenParse.Parsing;

internal class ParseResult<LexonType>
{
  public readonly ASTNode<LexonType> astNode;
  public readonly int lexonsConsumed;

  public ParseResult(ASTNode<LexonType> astNode, int lexonsConsumed)
  {
    this.astNode = astNode;
    this.lexonsConsumed = lexonsConsumed;
  }
}
