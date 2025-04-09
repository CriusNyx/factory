using SharpParse.Functional;

namespace Factory;

/// <summary>
/// Base class for all factory values.
/// This class has no implementation and is for organizational purposes.
/// All values in the Factory language that can store a value must be wrapped by a FactVal.
/// </summary>
public abstract class FactVal { }

public static class FactValExtensions
{
  public static (decimal quantity, string symbol) ToLimitVal(this FactVal factVal)
  {
    if (factVal is LimitVal limitVal)
    {
      var quantity = limitVal.value.value;
      var symbol = limitVal.identifier;
      return quantity.With(symbol);
    }
    throw new InvalidOperationException(
      $"Cannot convert FactVal of type {factVal.GetType()} to LimitVal"
    );
  }

  public static object ConvertToType(this FactVal factVal, Type type)
  {
    if (factVal.GetType() == type)
    {
      return factVal;
    }
    if (type == typeof(string) || type == typeof(String))
    {
      if (factVal is SymbolVal symbol)
      {
        return symbol.symbol;
      }
      if (factVal is NumVal numVal)
      {
        return numVal.value;
      }
      if (factVal is StringVal stringVal)
      {
        return stringVal.value;
      }
    }
    throw new NotImplementedException();
  }

  public static T To<T>(this FactVal factVal)
    where T : FactVal
  {
    return (factVal as T).NotNull();
  }
}
