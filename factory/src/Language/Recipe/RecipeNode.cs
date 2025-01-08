using GenParse.Functional;
using GenParse.Util;

namespace Factory;

[ASTClass("Recipe")]
public class RecipeNode : LanguageNode, ProgramExp
{
  [ASTField("symbol")]
  public SymbolNode name;

  [ASTField("RecipeExp*")]
  public RecipeExpNode[] expressions;

  public FactoryType CalculateType(TypeContext context)
  {
    foreach (var expression in expressions)
    {
      expression.CalculateType(context);
    }
    context.SetType(name.symbolName, new CSharpType(typeof(RecipeValue)));
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Void);
  }

  public (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var expressionValues = expressions.Map(x => x.Evaluate(ref context));

    var recipe = expressionValues.Reduce(
      new RecipeValue(name.symbolName),
      (factVal, recVal) => recVal.Amend((factVal as RecipeArgSet).NotNull())
    );
    context.GlobalValues.Add(recipe.recipeName, recipe);
    return recipe.With(context);
  }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren() => [name, .. expressions];
}
