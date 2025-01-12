using System.Reflection;
using GenParse.Functional;

namespace Factory;

public class MethodType(string name, FactoryType returnType, MethodArgumentType[] argumentTypes)
  : FactoryType
{
  public readonly string name = name;
  public readonly FactoryType returnType = returnType;
  public readonly MethodArgumentType[] argumentTypes = argumentTypes;

  public static MethodType FromCSharpMethod(MethodInfo methodInfo)
  {
    return new MethodType(
      $"{methodInfo.DeclaringType!.Name}.{methodInfo.Name}",
      FactoryType.FromCSharpType(methodInfo.ReturnType),
      MethodArgumentType.FromCSharpMethod(methodInfo)
    );
  }

  public bool CanAcceptValue(FactoryType other)
  {
    throw new NotImplementedException();
  }

  public (int argumentPosition, int arrayPosition)[] GenerateTypeMappings(
    FactoryType[] types,
    out bool[] success
  )
  {
    if (types.Length == 0)
    {
      success = [];
      return [];
    }

    List<(int, int)> output = new List<(int, int)>();
    success = types.Map(_ => false);

    var nonParamsArgs = argumentTypes.Filter(x => !x.paramsType);
    var paramsArgs = argumentTypes.Filter(x => x.paramsType);

    int inputTypesPointer = 0;

    for (int i = 0; i < nonParamsArgs.Length; i++)
    {
      var arg = nonParamsArgs[i];
      var type = types[inputTypesPointer];
      bool argAcceptsType = arg.argType.CanAcceptValue(type);

      if (argAcceptsType)
      {
        success[inputTypesPointer] = true;
        output.Add((i, -1));
        inputTypesPointer++;
      }
    }

    int[] arrLen = paramsArgs.Map(x => 0);

    for (int i = inputTypesPointer; i < types.Length; i++)
    {
      var type = types[i];
      for (int j = 0; j < paramsArgs.Length; j++)
      {
        var arg = paramsArgs[j];
        if (arg.argType.CanAcceptValue(type))
        {
          output.Add((nonParamsArgs.Length + j, arrLen[j]++));
          success[i] = true;
          break;
        }
      }
    }

    return output.ToArray();
  }

  public object[] MapArguments(FactVal[] values, FactoryType[] types)
  {
    var mapping = GenerateTypeMappings(types, out _);
    int[] lens = argumentTypes.Map(_ => 0);

    foreach (var (index, len) in mapping)
    {
      lens[index] = Math.Max(index, len + 1);
    }

    object[] output = new object[argumentTypes.Length];
    for (int i = 0; i < argumentTypes.Length; i++)
    {
      if (argumentTypes[i].paramsType)
      {
        output[i] = Array.CreateInstance(argumentTypes[i].argType.GetCSharpType(), lens[i]);
      }
    }

    foreach (var (value, map) in values.Zip(mapping))
    {
      var (argIndex, arrIndex) = map;
      if (arrIndex == -1)
      {
        var t = value.GetType().ToString();
        output[argIndex] = value;
      }
      else
      {
        (output[argIndex] as object[])![arrIndex] = value;
      }
    }
    return output!;
  }

  public bool[] CheckType(FactoryType[] types)
  {
    GenerateTypeMappings(types, out var output);
    return output;
  }

  public override string ToString()
  {
    return $"{InvocationString()}: {returnType.ToShortString()}";
  }

  public string InvocationString()
  {
    return $"{name}({string.Join(", ", argumentTypes.Map(x => x.ToShortString()))})";
  }

  public string ToShortString()
  {
    return InvocationString();
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
    var type = parameterInfo.ParameterType.IsArray
      ? parameterInfo.ParameterType.GetElementType()
      : parameterInfo.ParameterType;
    var factType = FactoryType.FromCSharpType(type!);
    return new MethodArgumentType(factType, optional, hasParamsType);
  }

  public static MethodArgumentType[] FromCSharpMethod(MethodInfo method)
  {
    return method.GetParameters().Map(x => FromParamInfo(x));
  }

  public override string ToString()
  {
    return $"MethodArgumentType({(optional ? "optional, " : "")}{(paramsType ? "params, " : "")}{argType}";
  }

  public string ToShortString()
  {
    return $"{(optional && !paramsType ? "optional " : "")}{(paramsType ? "params " : "")}{argType.ToShortString()}{(paramsType ? "[]" : "")}";
  }
}

[AttributeUsage(AttributeTargets.Parameter)]
public class ParamsAttribute : Attribute { }
