public class NumVal(decimal value) : FactVal
{
  public readonly decimal value = value;

  public override string ToString() => value.ToString();
}
