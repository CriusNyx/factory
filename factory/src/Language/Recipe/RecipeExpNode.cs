using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

public abstract class RecipeExpNode : LanguageNode, ValueNode
{
  public abstract ASTNode<FactoryLexon> astNode { get; }

  public abstract IEnumerable<Formatting.ITree<LanguageNode>> GetChildren();

  public abstract (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context);

  protected (FactVal value, ExecutionContext context) EvaluateKeywordSymbolArray(
    SymbolNode[] symbols,
    ExecutionContext context,
    ValType arrayType
  )
  {
    return new ArrayVal(symbols.Map(x => x.Evaluate())!)
      .Map(x =>
        x is SymbolVal symbolVal
          ? new TypedFactVal(arrayType, symbolVal)
          : throw new Exception($"Expected a symbol val but got a {x.GetType()} instead.")
      )
      .With(context);
  }

  public abstract FactoryType CalculateType(TypeContext context);
}
