using System.Net.Mime;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("symbol")]
public class SymbolNode : ValueNode
{
  public override ASTNode<FactoryLexon> astNode => _astNode;

  [AST]
  public ASTNode<FactoryLexon> _astNode { get; set; }
  public string symbolName => astNode.SourceCode();

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { };
  }

  public SymbolVal Evaluate()
  {
    return new SymbolVal(symbolName);
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    return Evaluate().With(context);
  }

  public override string ToString() => $"Symbol {astNode.SourceCode()}";

  public override FactoryType CalculateType(TypeContext context)
  {
    return context.GetType(symbolName);
  }
}
