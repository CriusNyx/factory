using CommandLine;
using Factory;
using Factory.Util;

public class LineArgSet(LineArgVal[] values) : FactVal
{
  public readonly LineArgVal[] values = values;
  public OutVal outVal => values.FirstOrDefault(x => x is OutVal).Cast<OutVal>().NotNull();
  public InVal[] inVals => values.FilterByType<LineArgVal, InVal>();
  public OutVal[] outVals => values.FilterByType<LineArgVal, OutVal>();
  public AltVal[] altVals => values.FilterByType<LineArgVal, AltVal>();
  public TallyVal[] tallyVals => values.FilterByType<LineArgVal, TallyVal>();
  public LimitVal[] limitVals => values.FilterByType<LineArgVal, LimitVal>();

  public static LineArgSet Join(LineArgSet a, LineArgSet b)
  {
    return new LineArgSet(a.values.Concat(b.values).Distinct().ToArray());
  }
}

public static class RecipeExpSetExtensions
{
  public static LineArgSet ToRecipeArgSet(this LineArgVal[] values)
  {
    return new LineArgSet(values);
  }
}
