namespace Factory;

public enum ValType
{
  input,
  output,
  alt,
  tally,
  limit,
}

public class TypedFactVal(ValType type, FactVal value) : FactVal
{
  public readonly ValType type = type;
  public readonly FactVal value = value;

  public override bool Equals(object? obj)
  {
    return obj is TypedFactVal val
      && type == val.type
      && EqualityComparer<FactVal>.Default.Equals(value, val.value);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(type, value);
  }

  public override string ToString()
  {
    return value.ToString()!;
  }
}
