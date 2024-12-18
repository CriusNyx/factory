using System.Reflection;

namespace Factory;

public class FuncVal(Func<ArrayVal, FactVal> func, Func<FactoryType[], bool[]> evaluateTypes)
  : FactVal
{
  public readonly Func<ArrayVal, FactVal> func = func;
  public readonly Func<FactoryType[], bool[]> evaluateTypes = evaluateTypes;

  [ExposeMember("Invoke")]
  public FactVal Invoke(ArrayVal arguments) => func.Invoke(arguments);

  public static FactVal? InvokeCSharpMethod(
    object target,
    MethodInfo methodInfo,
    ArrayVal arguments
  )
  {
    var methodArgs = methodInfo.GetParameters();
    if (methodArgs.Length == 0)
    {
      return methodInfo.Invoke(target, []) as FactVal;
    }
    else if (methodArgs.Length == 1)
    {
      return methodInfo.Invoke(target, [arguments]) as FactVal;
    }
    throw new NotImplementedException();
  }
}
