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

  public static ArrayVal AsArrayVal(this FactVal factVal)
  {
    if (factVal is ArrayVal arrayVal)
    {
      return arrayVal;
    }
    else
      throw new InvalidCastException($"Cannot convert {factVal.GetType()} to ArrayVal");
  }

  public static FactVal AsFactVal(this object value)
  {
    if (value is FactVal factVal)
    {
      return factVal;
    }
    throw new InvalidCastException($"Cannot convert {value.GetType()} to FactVal");
  }

  public static ArrayVal ToArrayVal(this FactVal[] factVal)
  {
    return new ArrayVal(factVal);
  }

  public static ArrayVal Map(this ArrayVal arrayVal, Func<FactVal, FactVal> func)
  {
    return new ArrayVal(arrayVal.array.Map(func));
  }

  public static ArrayVal Push(this ArrayVal arrVal, FactVal factVal)
  {
    return arrVal.array.Push(factVal).ToArrayVal();
  }

  public static ArrayVal PushRange(this ArrayVal arrVal, ArrayVal arrayVal)
  {
    return arrVal.array.Push(arrayVal.array).ToArrayVal();
  }

  public static ArrayVal PushOrReplace(
    this ArrayVal arrVal,
    FactVal other,
    Func<FactVal, bool> evaluate = null!
  )
  {
    return arrVal.array.PushOrReplace(other, evaluate).ToArrayVal();
  }

  public static ArrayVal Distinct(this ArrayVal arrVal)
  {
    return new ArrayVal(arrVal.array.Distinct().ToArray());
  }

  public static ArrayVal Filter(this ArrayVal arrVal, Func<FactVal, bool> func)
  {
    return new ArrayVal(arrVal.array.Filter(func));
  }

  public static ArrayVal FilterType(this ArrayVal arrVal, ValType type)
  {
    return arrVal.Filter(x => x is TypedFactVal typedVal && typedVal.type == type);
  }

  public static string[] ExtractSymbolsOfType(this ArrayVal arrVal, ValType type)
  {
    return arrVal
      .FilterType(type)
      .array.Map(x =>
        x is TypedFactVal typedVal && typedVal.value is SymbolVal symbolVal
          ? symbolVal.symbol
          : null
      )
      .FilterDefined();
  }
}
