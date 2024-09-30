using Factory.Parsing;
using GenParse.Functional;
using GenParse.Parsing;

public static class Executor
{
  public static object? Evaluate(ASTNode<FactoryLexon> astNode, ref ExecutionContext context)
  {
    object? output;
    (output, context) = Evaluate(astNode, context);
    return output;
  }

  public static (object? value, ExecutionContext context) Evaluate(
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
        return EvaluateKeywordSymbolArray(astNode, context, "outKeyword", ArrayType.outputArray);
      case "InExp":
        return EvaluateKeywordSymbolArray(astNode, context, "inKeyword", ArrayType.inputArray);
      case "AltExp":
        return EvaluateKeywordSymbolArray(astNode, context, "altKeyword", ArrayType.altArray);
      case "Recipe":
        return EvaluateRecipe(astNode, context);
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
        return EvaluateTallyExt(astNode, context);
      default:
        throw new NotImplementedException($"Not implemented for astNode {astNode.name}");
    }
  }

  static (object value, ExecutionContext context) EvaluateSymbol(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    RuntimeAssert.ASTNodeType(astNode, "symbol");
    return (astNode.SourceCode(), context);
  }

  static (object value, ExecutionContext context) EvaluateNumberLiteral(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    RuntimeAssert.ASTNodeType(astNode, "numberLiteral");
    return (decimal.Parse(astNode.SourceCode()), context);
  }

  static (object value, ExecutionContext context) EvaluateStar(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    return astNode.children.MapReduce(context, Evaluate);
  }

  static (object? value, ExecutionContext context) EvaluateKeywordSymbolArray(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context,
    string keyword,
    ArrayType arrayType
  )
  {
    var (_, symbols) = astNode.Match((keyword, "symbol*"));
    object? symbolsEvaluation;
    (symbolsEvaluation, context) = Evaluate(symbols!, context);
    return new AnnotatedArrayValue(arrayType, (object[])symbolsEvaluation!).With(context);
    throw new RuntimeAssertException("Expression did not match expected type");
  }

  static (object? value, ExecutionContext context) EvaluateSingleChild(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    return Evaluate(astNode.children[0], context);
  }

  public static (object? value, ExecutionContext context) EvaluateRecipe(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    var (_, nameNode, expressionsNodes) = astNode.Match(("recipeKeyword", "symbol", "RecipeExp*"));
    string name = (string)Evaluate(nameNode!, ref context)!;
    AnnotatedArrayValue[] expressionValues = Evaluate(expressionsNodes!, ref context)!
      .ToTypedArray<AnnotatedArrayValue>();

    string[] inputs = new string[] { };
    string[] outputs = new string[] { };
    string[] alts = new string[] { };

    foreach (var value in expressionValues)
    {
      switch (value.arrayType)
      {
        case ArrayType.inputArray:
          inputs = inputs.Push(value.array.ToTypedArray<string>());
          break;
        case ArrayType.outputArray:
          outputs = outputs.Push(value.array.ToTypedArray<string>());
          break;
        case ArrayType.altArray:
          alts = alts.Push(value.array.ToTypedArray<string>());
          break;
      }
    }

    var output = new RecipeValue(name, inputs, outputs, alts);
    context.GlobalValues[name] = output;
    return (output, context);
  }

  public static (object? value, ExecutionContext context) EvaluatePrint(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    var (_, valuesNodes) = astNode.Match(("printKeyword", "ValueExp*"));
    object[] values = (object[])Evaluate(valuesNodes!, ref context)!;
    foreach (var element in values)
    {
      context.standardOut.WriteLine(element);
      context.standardOut.WriteLine("");
    }

    return (null!, context);
  }

  public static (object? value, ExecutionContext contxet) EvaluateValueExp(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    if (astNode.TryMatch("symbol", out var symbol))
    {
      var symbolName = Evaluate(symbol, ref context) as string;
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
    else
    {
      throw new NotImplementedException();
    }
  }

  public static (object? value, ExecutionContext context) EvaluateInvocation(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    var (nameNode, invocationParamSetNode) = astNode.Match(("symbol", "InvocationParamSet"));
    var name = Evaluate(nameNode!, ref context);
    var functionDefinition = context.Resolve((string)name!);
    var invocationParams = (object[])Evaluate(invocationParamSetNode!, ref context)!;

    if (GetRecipeForInvocation(functionDefinition!) is RecipeValue recipeVal)
    {
      var invocationRequest = new RecipeSearchRequest(
        recipeVal,
        invocationParams.Map(RecipeSearchRequestArg.CreateArgFromRawValue)
      );

      return RecipeSearch.Search(invocationRequest).With(context);
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
        new string[] { },
        new string[] { recipe.primaryProduct.identifier },
        new string[] { }
      );
    }
    throw new InvalidOperationException($"Could not resolve invocation on object {o}");
  }

  public static (object? value, ExecutionContext context) EvaluateInvocationParamSet(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    if (astNode.TryMatch(("InvocationParam*", "FinalInvocationParam"), out var result))
    {
      var (invocationParams, finalParam) = result;
      var arr = Evaluate(invocationParams, ref context)!
        .ToTypedArray<object>()
        .Push(Evaluate(finalParam, ref context));
      return (arr, context);
    }
    return (new object[] { }, context);
  }

  public static (object? value, ExecutionContext context) EvaluateTallyExt(
    ASTNode<FactoryLexon> astNode,
    ExecutionContext context
  )
  {
    var (_, symbolNode) = astNode.Match(("tallyKeyword", "symbol"));
    var symbol = symbolNode!.SourceCode();
    return new RecipeSearchRequestTallyArg(symbol).With(context);
  }
}
