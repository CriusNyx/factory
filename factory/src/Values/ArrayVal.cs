public class ArrayVal : FactVal
{
  public readonly FactVal[] array;

  public ArrayVal(params FactVal[] array)
  {
    this.array = array;
  }
}
