using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("InExp")]
public class InExpNode(ASTNode<FactoryLexon> astNode) : RecipeExpNode, LanguageNode
{
  private ASTNode<FactoryLexon> _astNode = astNode;
  public override ASTNode<FactoryLexon> astNode => _astNode;

  [ASTField("symbol*")]
  public SymbolNode[] symbols;

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return EvaluateKeywordSymbolArray(symbols, context, ValType.input);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren() => symbols;

  public override FactoryType CalculateType(TypeContext context)
  {
    return FactoryType.InType;
  }
}
