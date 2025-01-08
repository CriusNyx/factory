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

  public static bool[] EvaluateAmendTypeValues(FactoryType[] argTypes) =>
    FactoryType.UnorderedTypeEvaluator(
      argTypes,
      FactoryType.NumberType,
      new RecipeType(RecipeTypeType.any)
    );

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

  public static bool[] EvaluateInvocationArgumentTypes(FactoryType[] factoryTypes)
  {
    return FactoryType.UnorderedTypeEvaluator(
      factoryTypes,
      new RecipeType(RecipeTypeType.any),
      FactoryType.NumberType
    );
  }

  [ExposeMember("Invoke")]
  public RecipeSearchResult Invoke(NumVal? numVal = null, [Params] RecipeArgSet[]? args = null)
  {
    return RecipeInvocation.InvokeRecipe(this, numVal?.value ?? 1, args ?? new FactVal[] { });
  }

  [ExposeMember("Test")]
  public RecipeSearchResult Test(decimal number, FactVal[] arguments)
  {
    throw new NotImplementedException();
  }

  [ExposeMember("Spread")]
  public RecipeArgSet Spread()
  {
    return this.arguments;
  }
}
