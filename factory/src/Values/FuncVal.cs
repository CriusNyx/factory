using System.Reflection;

namespace Factory;

public class FuncVal(Func<ArrayVal, FactVal> func) : FactVal, IFunc
{
  public readonly Func<ArrayVal, FactVal> func = func;

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
