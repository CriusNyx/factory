using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;

public class ProgramNode : LanguageNode
{
  public readonly ProgramExpNode[] children;

  public ProgramNode(
    Transformer<FactoryLexon, LanguageNode> transformer,
    ASTNode<FactoryLexon> astNode
  ) : base(transformer, astNode)
  {
    var star = astNode.Match("ProgramExp*")!;
    children = star.children.Map(x => ProgramExpNode.Transform(transformer, x));
  }

  public override string ToString()
  {
    return string.Join("", children.Map(x => x.ToString()));
  }
}
