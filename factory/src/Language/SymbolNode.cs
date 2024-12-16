using System.Net.Mime;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("symbol")]
public class SymbolNode : LanguageNode, ValueNode
{
  [AST]
  public ASTNode<FactoryLexon> astNode { get; set; }
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

  public FactoryType CalculateType(TypeContext context)
  {
    return context.GetType(symbolName);
  }
}
