using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

[ASTClass("Invocation")]
public class InvocationNode(ASTNode<FactoryLexon> astNode) : LanguageNode, ChainNode
{
  private ASTNode<FactoryLexon> _astNode = astNode;
  public ASTNode<FactoryLexon> astNode => _astNode;

  [ASTField("InvocationParamSet")]
  public ValueNode[] parameters;

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return parameters;
  }

  public FactVal Evaluate(FactVal target, ExecutionContext context)
  {
    var invocationParams = parameters.Map(x => x.Evaluate(ref context)).ToArrayVal();

    if (target is IFunc func)
    {
      return func.Invoke(invocationParams);
    }
    if (GetRecipeForInvocation(target) is RecipeValue recipeVal)
    {
      var invocation = invocationParams.array.Reduce(
        new InvocationContext(recipeVal),
        (factVal, context) => context.Amend(factVal)
      );
      return invocation.Invoke();
    }
    return null!;
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

  public string GetIdentifier()
  {
    throw new Exception("Invocation nodes cannot be converted to identifiers.");
  }
}
