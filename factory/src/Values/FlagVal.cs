public class BoolVal(bool value) : FactVal
{
  public readonly bool value = value;

  public override bool Equals(object? obj)
  {
    return obj is BoolVal val && value == val.value;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(value);
  }
}
