using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("AltExp")]
public class AltExpNode(ASTNode<FactoryLexon> astNode) : RecipeExpNode, LanguageNode
{
  private ASTNode<FactoryLexon> _astNode = astNode;
  public override ASTNode<FactoryLexon> astNode => _astNode;

  [ASTField("symbol*")]
  public SymbolNode[] symbols;

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return EvaluateKeywordSymbolArray(symbols, context, ValType.alt);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { };
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return FactoryType.AltType;
  }
}
