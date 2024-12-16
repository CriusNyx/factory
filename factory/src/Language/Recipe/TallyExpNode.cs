using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("TallyExp")]
public class TallyExpNode(ASTNode<FactoryLexon> astNode) : RecipeExpNode, LanguageNode
{
  private ASTNode<FactoryLexon> _astNode = astNode;
  public override ASTNode<FactoryLexon> astNode => _astNode;

  [ASTField("inlineKeyword?")]
  public bool inline;

  [ASTField("symbol*")]
  public SymbolNode[] symbols;

  public override string ToString()
  {
    return $"{base.ToString()} inline={inline}";
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return symbols
      .Map(x => x.Evaluate())
      .Map(x => new TypedFactVal(ValType.tally, new TallyVal(x.symbol, inline)))
      .ToArrayVal()
      .With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return symbols;
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return FactoryType.TallyType;
  }
}
