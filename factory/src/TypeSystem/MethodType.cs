using System.Reflection;
using GenParse.Functional;

namespace Factory;

public class MethodType(FactoryType returnType, MethodArgumentType[] argumentTypes) : FactoryType
{
  public readonly FactoryType returnType = returnType;
  public readonly MethodArgumentType[] argumentTypes = argumentTypes;

  public static MethodType FromCSharpMethod(MethodInfo methodInfo)
  {
    return new MethodType(
      FactoryType.FromCSharpType(methodInfo.ReturnType),
      MethodArgumentType.FromCSharpMethod(methodInfo)
    );
  }

  public bool CanAcceptValue(FactoryType other)
  {
    throw new NotImplementedException();
  }
}

public class MethodArgumentType(FactoryType argType, bool optional, bool paramsType)
{
  public readonly FactoryType argType = argType;
  public readonly bool optional = optional;
  public readonly bool paramsType = paramsType;

  public static MethodArgumentType FromParamInfo(ParameterInfo parameterInfo)
  {
    bool optional = parameterInfo.IsOptional;
    bool hasParamsType = parameterInfo.GetCustomAttribute<ParamsAttribute>() is ParamsAttribute;
    var factType = FactoryType.FromCSharpType(parameterInfo.ParameterType);
    return new MethodArgumentType(factType, hasParamsType, optional);
  }

  public static MethodArgumentType[] FromCSharpMethod(MethodInfo method)
  {
    return method.GetParameters().Map(x => FromParamInfo(x));
  }
}

[AttributeUsage(AttributeTargets.Parameter)]
public class ParamsAttribute : Attribute { }
