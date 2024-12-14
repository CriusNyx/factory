using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("symbol")]
public class SymbolNode(ASTNode<FactoryLexon> astNode) : LanguageNode, ValueNode
{
  private ASTNode<FactoryLexon> _astNode = astNode;
  public ASTNode<FactoryLexon> astNode => _astNode;
  public string symbolName => astNode.SourceCode();

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { };
  }

  public SymbolVal Evaluate()
  {
    return new SymbolVal(symbolName);
  }

  public (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return Evaluate().With(context);
  }

  public override string ToString() => $"Symbol {astNode.SourceCode()}";
}
