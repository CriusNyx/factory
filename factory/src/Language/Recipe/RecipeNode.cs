using SharpParse.Functional;
using SharpParse.Util;

namespace Factory;

[ASTClass("Recipe")]
public class RecipeNode : ProgramExp
{
  [ASTField("symbol")]
  public SymbolNode name;

  [ASTField("RecipeExp*")]
  public RecipeExpNode[] expressions;

  public override FactoryType CalculateType(TypeContext context)
  {
    name.OverrideType(new CSharpType(typeof(RecipeValue)));
    name.refInfo = new RefInfo(name.symbolName);
    foreach (var expression in expressions)
    {
      expression.GetFactoryType(context);
    }
    context.SetType(name.symbolName, new CSharpType(typeof(RecipeValue)));
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Void);
  }

  public override (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var expressionValues = expressions.Map(x => x.Evaluate(ref context));

    var recipe = expressionValues.Reduce(
      new RecipeValue(name.symbolName),
      (factVal, recVal) => recVal.Amend((factVal as RecipeArgSet).NotNull())
    );
    context.GlobalValues.Add(recipe.recipeName, recipe);
    return recipe.With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren() =>
    [name, .. expressions];

  public override (string?, string?) PrintSelf()
  {
    return ("recipe", null);
  }
}
