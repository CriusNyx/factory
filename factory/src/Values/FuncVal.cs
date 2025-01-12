using System.Reflection;

namespace Factory;

public class FuncVal(FactVal target, MethodInfo method, MethodType methodType) : FactVal
{
  public readonly FactVal target = target;
  public readonly MethodInfo method = method;
  public readonly MethodType methodType = methodType;

  public FactVal? Invoke(FactVal[] arguments, FactoryType[] types)
  {
    return method.Invoke(target, methodType.MapArguments(arguments, types)) as FactVal;
  }
}
