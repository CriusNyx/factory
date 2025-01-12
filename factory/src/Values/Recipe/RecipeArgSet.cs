using CommandLine;
using Factory;
using GenParse.Functional;

public class RecipeArgSet(RecipeArgVal[] values) : FactVal
{
  public readonly RecipeArgVal[] values = values;
  public OutVal outVal => values.FirstOrDefault(x => x is OutVal).Cast<OutVal>().NotNull();
  public InVal[] inVals => values.FilterByType<RecipeArgVal, InVal>();
  public OutVal[] outVals => values.FilterByType<RecipeArgVal, OutVal>();
  public AltVal[] altVals => values.FilterByType<RecipeArgVal, AltVal>();
  public TallyVal[] tallyVals => values.FilterByType<RecipeArgVal, TallyVal>();
  public LimitVal[] limitVals => values.FilterByType<RecipeArgVal, LimitVal>();

  public static RecipeArgSet Join(RecipeArgSet a, RecipeArgSet b)
  {
    return new RecipeArgSet(a.values.Concat(b.values).Distinct().ToArray());
  }
}

public static class RecipeExpSetExtensions
{
  public static RecipeArgSet ToRecipeArgSet(this RecipeArgVal[] values)
  {
    return new RecipeArgSet(values);
  }
}
