using GenParse.Functional;

public class ArrayVal : FactVal
{
  public readonly FactVal[] array;

  public ArrayVal(params FactVal[] array)
  {
    this.array = array;
  }

  public override string ToString()
  {
    return string.Join(" ", array.Map(x => x.ToString()));
  }
}
