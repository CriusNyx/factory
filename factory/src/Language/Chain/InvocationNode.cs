using System.Reflection;
using SharpParse.Functional;
using SharpParse.Parsing;
using SharpParse.Util;

namespace Factory;

[ASTClass("Invocation")]
public class InvocationNode : ChainNode
{
  [ASTField("InvocationParamSet")]
  public ValueNode[] parameters;

  private FactoryType[] argumentTypes;
  private MethodType methodType;

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return parameters;
  }

  public override FactVal Evaluate(FactVal target, ExecutionContext context)
  {
    var invocationParams = parameters.Map(x => x.Evaluate(ref context));
    if (target is FuncVal funcVal)
    {
      return funcVal.Invoke(invocationParams, argumentTypes)!;
    }
    else
    {
      var invocationMethod = target.GetType().GetFactoryInvocationMethod();
      var methodType = MethodType.FromCSharpMethod(invocationMethod);
      var mappedParams = methodType.MapArguments(invocationParams, argumentTypes);
      return (invocationMethod?.Invoke(target, mappedParams) as FactVal)!;
    }
  }

  public override string GetIdentifier()
  {
    throw new Exception("Invocation nodes cannot be converted to identifiers.");
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    // Get the parent
    refType = context.Peek().ResolveType(context);
    argumentTypes = parameters.Map(x => x.GetFactoryType(context));

    if (refType is CSharpType cSharpType)
    {
      var type = cSharpType.type;

      var invocationMethod = type.GetFactoryInvocationMethod();

      methodType = MethodType.FromCSharpMethod(invocationMethod);
    }
    if (refType is MethodType mt)
    {
      methodType = mt;
    }

    if (methodType == null)
    {
      var pos = astNode.CalculatePosition();
      context.AddError(
        pos.start,
        pos.length,
        $"{refType.ToShortString()} cannot be invoked as a method."
      );
      return null!;
    }

    var mapping = methodType.GenerateTypeMappings(argumentTypes, out var succ);
    foreach (var (success, type, param) in succ.Zip(argumentTypes, parameters))
    {
      if (!success)
      {
        var errorPos = param.astNode.CalculatePosition();
        context.AddError(
          errorPos.start,
          errorPos.length,
          $"Argument of type {type.ToShortString()} is not valid for {methodType.name}({string.Join(", ", methodType.argumentTypes.Map(x => x.ToShortString()))})."
        );
      }
    }

    return methodType.returnType;
  }

  public override (string?, string?) PrintSelf()
  {
    return ("(", ")");
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

  public static MethodInfo GetFactorySpreadMethod(this Type type)
  {
    foreach (var method in type.GetMethods())
    {
      if (
        method.GetCustomAttribute<ExposeMemberAttribute>() is ExposeMemberAttribute expose
        && expose.name == "Spread"
      )
      {
        return method;
      }
    }
    return null!;
  }
}
