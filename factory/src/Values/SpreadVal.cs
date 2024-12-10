using Microsoft.CodeAnalysis.CSharp.Syntax;

public class SpreadVal(ArrayVal arr) : FactVal, IUnfold
{
  public readonly ArrayVal arr = arr;

  public FactVal[] Unfold()
  {
    return arr.array;
  }
}
