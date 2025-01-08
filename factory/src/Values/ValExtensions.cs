using GenParse.Functional;

namespace Factory;

public static class ValExtensions
{
  public static SymbolVal ToSymbolVal(this string str)
  {
    return new SymbolVal(str);
  }

  public static SymbolVal AsSymbolVal(this FactVal factVal)
  {
    if (factVal is SymbolVal symbolVal)
    {
      return symbolVal;
    }
    else
      throw new InvalidCastException($"Cannot convert {factVal.GetType()} to SymbolVal");
  }

  public static NumVal ToNumVal(this decimal num)
  {
    return new NumVal(num);
  }

  public static FactVal AsFactVal(this object value)
  {
    if (value is FactVal factVal)
    {
      return factVal;
    }
    throw new InvalidCastException($"Cannot convert {value.GetType()} to FactVal");
  }
}
