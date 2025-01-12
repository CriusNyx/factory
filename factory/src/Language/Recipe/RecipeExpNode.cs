using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

public abstract class RecipeExpNode : LanguageNode, ValueNode
{
  public abstract ASTNode<FactoryLexon> astNode { get; }

  public abstract IEnumerable<Formatting.ITree<LanguageNode>> GetChildren();

  public abstract (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context);

  public abstract FactoryType CalculateType(TypeContext context);
}
