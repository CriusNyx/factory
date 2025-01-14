using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("TallyExp")]
public class TallyExpNode(ASTNode<FactoryLexon> astNode) : RecipeExpNode
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
    return symbols.Map(x => new TallyVal(x.symbolName, inline)).ToRecipeArgSet().With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return symbols;
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    return FactoryType.FromCSharpType(typeof(RecipeArgSet));
  }

  public override (string?, string?) PrintSelf()
  {
    string output = "tally";
    if (inline)
    {
      output += " inline";
    }
    return (output, null);
  }
}
