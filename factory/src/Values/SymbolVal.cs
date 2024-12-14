namespace Factory;

public class SymbolVal(string symbol) : FactVal
{
  public readonly string symbol = symbol;

  public override bool Equals(object? obj)
  {
    return obj is SymbolVal val && symbol == val.symbol;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(symbol);
  }

  public override string ToString() => symbol.ToString();
}
