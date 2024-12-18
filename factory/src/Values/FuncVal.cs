using System.Reflection;

namespace Factory;

public class FuncVal(FactVal target, MethodInfo method, MethodType methodType) : FactVal
{
  public readonly FactVal target = target;
  public readonly MethodInfo method = method;
  public readonly MethodType methodType = methodType;

  [ExposeMember("Invoke")]
  public FactVal? Invoke(FactVal[] arguments)
  {
    return method.Invoke(target, [arguments]) as FactVal;
  }
}
