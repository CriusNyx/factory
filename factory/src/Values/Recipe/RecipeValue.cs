using System.Diagnostics;
using System.Text;
using GenParse.Functional;

namespace Factory;

public class RecipeValue(string recipeName, RecipeArgSet? arguments = null) : FactVal
{
  [ExposeMember("Name")]
  public string recipeName = recipeName.NotNull();
  public readonly RecipeArgSet arguments = arguments ?? new RecipeArgSet([]);

  public override string ToString()
  {
    var builder = new StringBuilder();

    var inVals = arguments.values.Map(x => x as InVal).FilterDefined();
    var outVals = arguments.values.Map(x => x as OutVal).FilterDefined();
    var altVals = arguments.values.Map(x => x as AltVal).FilterDefined();
    var tallyVals = arguments.values.Map(x => x as TallyVal).FilterDefined();

    var outlineTallyVals = tallyVals.Filter(x => !x.inline);
    var inlineTallyVals = tallyVals.Filter(x => x.inline);

    builder.AppendLine($"recipe {recipeName}");
    builder.AppendLine($"  in {string.Join(" ", inVals.Map(x => x.ToString()))}");
    builder.AppendLine($"  out {string.Join(" ", outVals.Map(x => x.ToString()))}");
    if (altVals.Length > 0)
    {
      builder.AppendLine($"  alt {string.Join(" ", altVals.Map(x => x.ToString()))}");
    }
    if (outlineTallyVals.Length > 0)
    {
      builder.AppendLine($"  tally {string.Join(" ", outlineTallyVals.Map(x => x.identifier))}");
    }
    if (inlineTallyVals.Length > 0)
    {
      builder.AppendLine(
        $"  tally inline {string.Join(" ", inlineTallyVals.Map(x => x.identifier))}"
      );
    }
    return builder.ToString();
  }

  [ExposeMember("Amend")]
  public RecipeValue AmendInvocation([Params] FactVal[] args)
  {
    return args.FilterByType<FactVal, RecipeArgSet>()
      .Reduce(this, (element, recipe) => recipe.Amend(element));
  }

  public RecipeValue Amend(RecipeArgSet factVal)
  {
    return new RecipeValue(recipeName, RecipeArgSet.Join(arguments, factVal));
  }

#if DEBUG
  [ExposeMember("Break")]
  public void Break()
  {
    Debugger.Break();
  }
#endif

  [ExposeMember("Invoke")]
  public RecipeSolution Invoke(NumVal? numVal = null, [Params] RecipeArgSet[]? args = null)
  {
    return RecipeInvocation.InvokeRecipe(this, numVal?.value ?? 1, args ?? new FactVal[] { });
  }

#if DEBUG
  [ExposeMember("Test")]
  public RecipeSolution Test(decimal number, FactVal[] arguments)
  {
    throw new NotImplementedException();
  }
#endif

  [ExposeMember("Spread")]
  public RecipeArgSet Spread()
  {
    return this.arguments;
  }
}
