using Factory.Parsing;
using GenParse.Parsing;

public abstract class LanguageNode
{
  public readonly ASTNode<FactoryLexon> astNode;

  protected LanguageNode(
    Transformer<FactoryLexon, LanguageNode> transformer,
    ASTNode<FactoryLexon> astNode
  )
  {
    this.astNode = astNode;
  }
}
