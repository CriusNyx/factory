using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("Recipe")]
public class RecipeNode : ProgramExp
{
  public override ASTNode<FactoryLexon> astNode => _astNode;

  [AST]
  public ASTNode<FactoryLexon> _astNode { get; set; }

  [ASTField("symbol")]
  public SymbolNode name;

  [ASTField("RecipeExp*")]
  public RecipeExpNode[] expressions;

  public override FactoryType CalculateType(TypeContext context)
  {
    foreach (var expression in expressions)
    {
      expression.CalculateType(context);
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
}
