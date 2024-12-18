using CommandLine;
using GenParse.Functional;

namespace Factory;

public class MethodType(Type outType, ArgumentTypeEvaluatorAttribute typeEvaluator) : FactoryType
{
  public readonly Type outType = outType;
  public readonly ArgumentTypeEvaluatorAttribute typeEvaluator = typeEvaluator;

  public bool CanAcceptValue(FactoryType other)
  {
    return other is MethodType methodType && methodType.outType == outType;
  }
}

[AttributeUsage(AttributeTargets.Method)]
public class ArgumentTypeEvaluatorAttribute(Type typeOwner, string evaluationMethod) : Attribute
{
  public readonly Type typeOwner = typeOwner;
  public readonly string evaluationMethod = evaluationMethod;

  public bool[] CheckTypes(FactoryType[] factoryTypes)
  {
    var method = typeOwner.GetMethod(evaluationMethod).NotNull();
    return method.Invoke(null, [factoryTypes]).Cast<bool[]>();
  }
}
