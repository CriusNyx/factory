using GenParse.Functional;

namespace Factory;

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

  public ArrayVal UnfoldAll()
  {
    List<FactVal> newArr = new List<FactVal>();
    foreach (var element in array)
    {
      if (element is IUnfold unfold)
      {
        newArr.AddRange(unfold.Unfold());
      }
      else
      {
        newArr.Add(element);
      }
    }
    return new ArrayVal(newArr.ToArray());
  }
}
