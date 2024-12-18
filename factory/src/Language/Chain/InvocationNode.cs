using System.Reflection;
using CommandLine;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("Invocation")]
public class InvocationNode : LanguageNode, ChainNode
{
  [AST]
  public ASTNode<FactoryLexon> ast;

  [ASTField("InvocationParamSet")]
  public ValueNode[] parameters;

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return parameters;
  }

  public FactVal Evaluate(FactVal target, ExecutionContext context)
  {
    var invocationParams = parameters.Map(x => x.Evaluate(ref context)).ToArrayVal();

    var invocationMethod = target.GetType().GetFactoryInvocationMethod();
    return (invocationMethod?.Invoke(target, [invocationParams]) as FactVal)!;
  }

  public string GetIdentifier()
  {
    throw new Exception("Invocation nodes cannot be converted to identifiers.");
  }

  public FactoryType CalculateType(TypeContext context)
  {
    var current = context.Peek().Resolve(context);
    var argTypes = parameters.Map(x => x.CalculateType(context));

    if (current is CSharpType cSharpType)
    {
      var type = cSharpType.type;

      var invocationMethod = type.GetFactoryInvocationMethod();

      var typeEvaluator = invocationMethod
        .GetCustomAttribute<ArgumentTypeEvaluatorAttribute>()
        .NotNull();
      CheckArgsForErrors(argTypes, typeEvaluator.CheckTypes, context);

      if (invocationMethod.ReturnType != null)
      {
        return FactoryType.FromCSharpType(invocationMethod.ReturnType);
      }
    }
    if (current is MethodType methodType)
    {
      var type = methodType.outType;
      CheckArgsForErrors(argTypes, methodType.typeEvaluator.CheckTypes, context);

      return FactoryType.FromCSharpType(type);
    }
    var pos = ast.CalculatePosition();
    context.AddError(pos.start, pos.length, $"Cannot invoke on type {current}");
    return FactoryType.VoidType;
  }

  private void CheckArgsForErrors(
    FactoryType[] argTypes,
    Func<FactoryType[], bool[]> evaluator,
    TypeContext typeContext
  )
  {
    var typesAreValid = evaluator(argTypes);

    foreach (var (parameter, argType, valid) in parameters.Zip(argTypes, typesAreValid))
    {
      if (!valid)
      {
        var argErrorPos = parameter.astNode.CalculatePosition();
        typeContext.AddError(
          argErrorPos.start,
          argErrorPos.length,
          $"Argument of type {argType} is not valid."
        );
      }
    }
  }
}

public static class InvocationExtensions
{
  public static MethodInfo GetFactoryInvocationMethod(this Type type)
  {
    foreach (var method in type.GetMethods())
    {
      if (
        method.GetCustomAttribute<ExposeMemberAttribute>() is ExposeMemberAttribute expose
        && expose.name == "Invoke"
      )
      {
        return method;
      }
    }
    return null!;
  }
}
