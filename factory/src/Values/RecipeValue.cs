using System.Text;
using GenParse.Functional;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class RecipeValue(string recipeName, ArrayVal? arguments = null) : FactVal
{
  public readonly string recipeName = recipeName.NotNull();
  public readonly ArrayVal arguments = arguments?.Distinct() ?? new ArrayVal();

  public override string ToString()
  {
    var builder = new StringBuilder();

    var inVals = arguments.FilterType(ValType.input);
    var outVals = arguments.FilterType(ValType.output);
    var altVals = arguments.FilterType(ValType.alt);
    var tallyVals = arguments.FilterType(ValType.tally).array.Map(x => x as TypedFactVal).Map(x => x!.value as TallyVal).FilterDefined().NotNull();
    var outlineTallyVals = tallyVals.Filter(x => !x.inline);
    var inlineTallyVals = tallyVals.Filter(x => x.inline);

    builder.AppendLine($"recipe {recipeName}");
    builder.AppendLine($"  in {inVals}");
    builder.AppendLine($"  out {outVals}");
    if(altVals.array.Length > 0){
      builder.AppendLine($"  alt {altVals}");
    }
    if(outlineTallyVals.Length > 0){
      builder.AppendLine($"  tally {string.Join(" ", outlineTallyVals.Map(x => x.symbol))}");
    }
    if(inlineTallyVals.Length > 0){
      builder.AppendLine($"  tally inline {string.Join(" ", inlineTallyVals.Map(x => x.symbol))}");
    }
    return builder.ToString();
  }

  public RecipeValue Amend(FactVal factVal)
  {
    if(factVal is ArrayVal arrayVal){
      return Clone(arguments: arguments.PushRange(arrayVal));
    }
    else if(factVal is TypedFactVal typedVal){
      return Clone(arguments: arguments.Push(typedVal));
    }
    return this;
  }

  public RecipeValue Clone(string? recipeName = null, ArrayVal? arguments = null)
  {
    return new RecipeValue(
      recipeName ?? this.recipeName,
      arguments ?? this.arguments
    );
  }

  private static ValType[] valTypesThatModifyRecipeVal = new ValType[]
  {
    ValType.input,
    ValType.output,
    ValType.alt,
    ValType.tally
  };

  public static bool FactValModifiesRecipeVal(FactVal factVal)
  {
    if (factVal is TypedFactVal typedVal)
    {
      return valTypesThatModifyRecipeVal.Contains(typedVal.type);
    }
    else if(factVal is ArrayVal arrayVal){
      return arrayVal.array.All(FactValModifiesRecipeVal);
    }
    return false;
  }
}
