using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("Invocation")]
public class InvocationNode(ASTNode<FactoryLexon> astNode) : LanguageNode, ValueNode
{
  private ASTNode<FactoryLexon> _astNode = astNode;
  public ASTNode<FactoryLexon> astNode => _astNode;

  [ASTField("symbol")]
  public SymbolNode invocationTarget;

  [ASTField("InvocationParamSet")]
  public ValueNode[] parameters;

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return parameters;
  }

  public (FactVal value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var name = invocationTarget.Evaluate();
    var functionDefinition = context.Resolve(name).NotNull();
    var invocationParams = parameters.Map(x => x.Evaluate(ref context)).ToArrayVal();

    if (GetRecipeForInvocation(functionDefinition) is RecipeValue recipeVal)
    {
      var invocation = invocationParams.array.Reduce(
        new InvocationContext(recipeVal),
        (factVal, context) => context.Amend(factVal)
      );
      return invocation.Invoke().With(context);
    }
    return (null!, context);
  }

  static RecipeValue GetRecipeForInvocation(object o)
  {
    if (o is RecipeValue recipeValue)
    {
      return recipeValue;
    }
    else if (o is Recipe recipe)
    {
      return new RecipeValue(
        recipe.identifier,
        new ArrayVal(
          new TypedFactVal(ValType.output, new SymbolVal(recipe.primaryProduct.identifier))
        )
      );
    }
    throw new InvalidOperationException($"Could not resolve invocation on object {o}");
  }
}
