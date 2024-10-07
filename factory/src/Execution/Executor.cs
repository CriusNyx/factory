using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;

public static class Executor
{
  public static FactVal? Evaluate(ASTNode<FactoryLexon> astNode, ref ExecutionContext context)
  {
    FactVal? output;
    (output, context) = Evaluate(astNode, context);
    return output;
  }

  public static (FactVal? value, ExecutionContext context) Evaluate(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    switch (astNode.name)
    {
      case "symbol":
        return EvaluateSymbol(astNode, context);
      case "numberLiteral":
        return EvaluateNumberLiteral(astNode, context);
      case "symbol*":
      case "RecipeExp*":
      case "ProgramExp*":
      case "ValueExp*":
      case "InvocationParam*":
        return EvaluateStar(astNode, context);
      case "OutExp":
        return EvaluateKeywordSymbolArray(astNode, context, "outKeyword", ValType.output);
      case "InExp":
        return EvaluateKeywordSymbolArray(astNode, context, "inKeyword", ValType.input);
      case "AltExp":
        return EvaluateKeywordSymbolArray(astNode, context, "altKeyword", ValType.alt);
      case "Recipe":
        return EvaluateRecipeExp(astNode, context);
      case "RecipeExp":
      case "ProgramExp":
      case "Program":
      case "InvocationParam":
      case "FinalInvocationParam":
      case "Literal":
        return EvaluateSingleChild(astNode, context);
      case "PrintExp":
        return EvaluatePrint(astNode, context);
      case "ValueExp":
        return EvaluateValueExp(astNode, context);
      case "InvocationParamSet":
        return EvaluateInvocationParamSet(astNode, context);
      case "Invocation":
        return EvaluateInvocation(astNode, context);
      case "TallyExp":
        return EvaluateTallyExp(astNode, context);
      case "AssignExp":
        return EvaluateAssignExp(astNode, context);
      default:
        throw new NotImplementedException($"Not implemented for astNode {astNode.name}");
    }
  }

  static (FactVal value, ExecutionContext context) EvaluateSymbol(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    RuntimeAssert.ASTNodeType(astNode, "symbol");
    return astNode.SourceCode().ToSymbolVal().With(context);
  }

  static (FactVal value, ExecutionContext context) EvaluateNumberLiteral(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    RuntimeAssert.ASTNodeType(astNode, "numberLiteral");
    return decimal.Parse(astNode.SourceCode()).ToNumVal().With(context);
  }

  static (FactVal value, ExecutionContext context) EvaluateStar(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    return astNode.children
      .Map((x) => Evaluate(x, ref context))
      .FilterDefined()
      .ToArrayVal()
      .With(context);
  }

  static (FactVal? value, ExecutionContext context) EvaluateKeywordSymbolArray(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context,
    string keyword,
    ValType arrayType
  )
  {
    var (_, symbols) = astNode.Match((keyword, "symbol*"));

    return Evaluate(symbols!, ref context)!
      .AsArrayVal()
      .Map(
        x =>
          x is SymbolVal symbolVal
            ? new TypedFactVal(arrayType, symbolVal)
            : throw new Exception($"Expected a symbol val but got a {x.GetType()} instead.")
      )
      .With(context);
  }

  static (FactVal? value, ExecutionContext context) EvaluateSingleChild(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    return Evaluate(astNode.children[0], context);
  }

  public static (FactVal? value, ExecutionContext context) EvaluateRecipeExp(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    var (_, nameNode, expressionsNodes) = astNode.Match(("recipeKeyword", "symbol", "RecipeExp*"));
    SymbolVal? name = Evaluate(nameNode!, ref context) as SymbolVal;
    ArrayVal expressionValues = Evaluate(expressionsNodes!, ref context)!.AsArrayVal();

    var recipe = expressionValues.array.Reduce(
      new RecipeValue(name?.symbol ?? ""),
      (factVal, recVal) => recVal.Amend(factVal)
    );
    context.GlobalValues.Add(recipe.recipeName, recipe);
    return recipe.With(context);
  }

  public static (FactVal? value, ExecutionContext context) EvaluatePrint(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    var (_, valuesNodes) = astNode.Match(("printKeyword", "ValueExp*"));
    ArrayVal values = (ArrayVal)Evaluate(valuesNodes!, ref context)!;
    foreach (var element in values.array)
    {
      context.standardOut.WriteLine(element);
      context.standardOut.WriteLine("");
    }

    return (null!, context);
  }

  public static (FactVal? value, ExecutionContext contxet) EvaluateValueExp(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    if (astNode.TryMatch("symbol", out var symbol))
    {
      SymbolVal symbolName = (SymbolVal)Evaluate(symbol, ref context)!;
      return (context.Resolve(symbolName!), context);
    }
    else if (astNode.TryMatch("Literal", out var literalNode))
    {
      return Evaluate(literalNode, context);
    }
    else if (astNode.TryMatch("Invocation", out var invocation))
    {
      return Evaluate(invocation, context);
    }
    else if (astNode.TryMatch("TallyExp", out var tallyNode))
    {
      return Evaluate(tallyNode, context);
    }
    else if (astNode.TryMatch("RecipeExp", out var recipeExpNode))
    {
      return Evaluate(recipeExpNode, context);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  public static (FactVal? value, ExecutionContext context) EvaluateInvocation(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    var (nameNode, invocationParamSetNode) = astNode.Match(("symbol", "InvocationParamSet"));
    var name = (SymbolVal)Evaluate(nameNode.NotNull(), ref context)!;
    var functionDefinition = context.Resolve(name).NotNull();
    var invocationParams = (ArrayVal)Evaluate(invocationParamSetNode!, ref context)!;

    if (GetRecipeForInvocation(functionDefinition) is RecipeValue recipeVal)
    {
      var invocation = invocationParams.array.Reduce(
        new InvocationContext(recipeVal),
        (factVal, context) => context.Amend(factVal)
      );
      return invocation.Invoke().With(context);
    }
    return (null, context);
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

  public static (FactVal? value, ExecutionContext context) EvaluateInvocationParamSet(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    if (astNode.TryMatch(("InvocationParam*", "FinalInvocationParam"), out var result))
    {
      var (invocationParams, finalParam) = result;

      return Evaluate(invocationParams, ref context)!
        .AsArrayVal()
        .Push(Evaluate(finalParam, ref context)!)
        .With(context);
    }
    return (new ArrayVal(), context);
  }

  public static (FactVal? value, ExecutionContext context) EvaluateTallyExp(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    var (_, inlineNode, symbolNode) = astNode.Match(("tallyKeyword", "inlineKeyword?", "symbol*"));
    var inline = inlineNode!.children.Length > 0;
    return Evaluate(symbolNode.NotNull(), ref context)
      .NotNull()
      .AsArrayVal()
      .NotNull()
      .Map(
        x =>
          x is SymbolVal symbolVal
            ? new TypedFactVal(ValType.tally, new TallyVal(symbolVal.symbol, inline))
            : throw new InvalidCastException("Unknown grammar")
      )
      .With(context);
  }

  public static (FactVal? value, ExecutionContext context) EvaluateAssignExp(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    var (symbolNode, _, valNode) = astNode.Match(("symbol", "equalSign", "ValueExp"));
    var symbol = symbolNode!.SourceCode();
    var value = Evaluate(valNode.NotNull(), ref context);
    if (value != null)
    {
      context.GlobalValues[symbol] = value;
    }
    return value.With(context);
  }
}
