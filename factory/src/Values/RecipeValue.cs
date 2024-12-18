using System.Diagnostics;
using System.Text;
using GenParse.Functional;

namespace Factory;

public class RecipeValue(string recipeName, ArrayVal? arguments = null) : FactVal, ISpread
{
  [ExposeMember("Name")]
  public string recipeName = recipeName.NotNull();
  public readonly ArrayVal arguments = arguments?.Distinct() ?? new ArrayVal();

  public override string ToString()
  {
    var builder = new StringBuilder();

    var inVals = arguments.FilterType(ValType.input);
    var outVals = arguments.FilterType(ValType.output);
    var altVals = arguments.FilterType(ValType.alt);
    var tallyVals = arguments
      .FilterType(ValType.tally)
      .array.Map(x => x as TypedFactVal)
      .Map(x => x!.value as TallyVal)
      .FilterDefined()
      .NotNull();
    var outlineTallyVals = tallyVals.Filter(x => !x.inline);
    var inlineTallyVals = tallyVals.Filter(x => x.inline);

    builder.AppendLine($"recipe {recipeName}");
    builder.AppendLine($"  in {inVals}");
    builder.AppendLine($"  out {outVals}");
    if (altVals.array.Length > 0)
    {
      builder.AppendLine($"  alt {altVals}");
    }
    if (outlineTallyVals.Length > 0)
    {
      builder.AppendLine($"  tally {string.Join(" ", outlineTallyVals.Map(x => x.symbol))}");
    }
    if (inlineTallyVals.Length > 0)
    {
      builder.AppendLine($"  tally inline {string.Join(" ", inlineTallyVals.Map(x => x.symbol))}");
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
  [ArgumentTypeEvaluator(typeof(RecipeValue), nameof(EvaluateAmendTypeValues))]
  public RecipeValue AmendInvocation(ArrayVal arrayVal)
  {
    return arrayVal.array.Reduce(this, (element, recipe) => recipe.Amend(element));
  }

  public RecipeValue Amend(FactVal factVal)
  {
    if (factVal is ArrayVal arrayVal)
    {
      return Clone(
        arguments: arrayVal.array.Reduce(
          arguments,
          (amendment, arguments) =>
            arguments.PushOrReplace(amendment, (other) => CompareValType(amendment, other))
        )
      );
    }
    else if (factVal is IUnfold unfold)
    {
      return Clone(
        arguments: unfold
          .Unfold()
          .Reduce(
            arguments,
            (amendment, arguments) =>
              arguments.PushOrReplace(amendment, (other) => CompareValType(amendment, other))
          )
      );
    }
    else if (factVal is TypedFactVal typedVal)
    {
      return Clone(arguments: arguments.Push(typedVal));
    }
    return this;
  }

#if DEBUG
  [ExposeMember("Break")]
  public void Break()
  {
    Debugger.Break();
  }
#endif

  private bool CompareValType(FactVal o, FactVal other)
  {
    if (o is TypedFactVal typedVal)
    {
      var otherTypedVal = other as TypedFactVal;
      if (otherTypedVal == null)
      {
        return false;
      }
      if (typedVal.type != otherTypedVal.type)
      {
        return false;
      }
      switch (typedVal.type)
      {
        case ValType.alt:
        case ValType.output:
        case ValType.input:
        case ValType.tally:
          return typedVal.value.Equals(otherTypedVal.value);
        case ValType.limit:
        {
          var (_, a) = (typedVal.value as PairVal).NotNull();
          var (_, b) = (typedVal.value as PairVal).NotNull();
          return a.Equals(b);
        }
      }
    }
    throw new NotImplementedException();
  }

  public RecipeValue Clone(string? recipeName = null, ArrayVal? arguments = null)
  {
    return new RecipeValue(recipeName ?? this.recipeName, arguments ?? this.arguments);
  }

  private static ValType[] valTypesThatModifyRecipeVal =
  [
    ValType.input,
    ValType.output,
    ValType.alt,
    ValType.tally,
    ValType.limit,
  ];

  public static bool FactValModifiesRecipeVal(FactVal factVal)
  {
    if (factVal is TypedFactVal typedVal)
    {
      return valTypesThatModifyRecipeVal.Contains(typedVal.type);
    }
    else if (factVal is ArrayVal arrayVal)
    {
      return arrayVal.array.All(FactValModifiesRecipeVal);
    }
    return false;
  }

  public ArrayVal Spread()
  {
    return arguments;
  }

  public static bool[] EvaluateInvocationArgumentTypes(FactoryType[] factoryTypes)
  {
    return FactoryType.UnorderedTypeEvaluator(
      factoryTypes,
      new RecipeType(RecipeTypeType.any),
      FactoryType.NumberType
    );
  }

  [ExposeMember("Invoke")]
  [ArgumentTypeEvaluator(typeof(RecipeValue), nameof(EvaluateInvocationArgumentTypes))]
  public RecipeSearchResult Invoke(ArrayVal arguments)
  {
    return RecipeInvocation.InvokeRecipe(this, arguments);
  }
}
