using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("SpreadExp")]
public class SpreadExpNode(ASTNode<FactoryLexon> astNode) : RecipeExpNode, LanguageNode
{
  private ASTNode<FactoryLexon> _astNode = astNode;
  public override ASTNode<FactoryLexon> astNode => _astNode;

  [ASTField("symbol")]
  public SymbolNode symbol;

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var target = context.Resolve(symbol.Evaluate());
    if (target is ISpread spread)
    {
      return new SpreadVal(spread.Spread()).With(context);
    }
    throw new NotImplementedException("Cannot spread on a value that is not spreadable.");
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { symbol };
  }
}
