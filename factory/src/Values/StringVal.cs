public class StringVal(string value) : FactVal
{
  public readonly string value = value;

  public override string ToString()
  {
    return value;
  }
}
