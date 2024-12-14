using GenParse.Functional;

namespace Factory;

/// <summary>
/// Base class for all factory values.
/// This class has no implementation and is for organizational purposes.
/// All values in the Factory language that can store a value must be wrapped by a FactVal.
/// </summary>
public abstract class FactVal { }

public static class FactValExtensions
{
  public static bool IsLimitVal(this FactVal factVal)
  {
    return factVal is TypedFactVal typedVal && typedVal.type == ValType.limit;
  }

  public static (decimal quantity, string symbol) ToLimitVal(this FactVal factVal)
  {
    if (IsLimitVal(factVal))
    {
      var pairVal = ((factVal as TypedFactVal).NotNull().value as PairVal).NotNull();
      var (numVal, symbolVal) = pairVal!;
      var quantity = (numVal as NumVal).NotNull().value;
      var symbol = (symbolVal as SymbolVal).NotNull().symbol;
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
}
