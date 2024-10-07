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
    builder.AppendLine($"recipe {recipeName}");
    builder.AppendLine($"  in {string.Join(" ", arguments.FilterType(ValType.input))}");
    builder.AppendLine($"  out {string.Join(" ", arguments.FilterType(ValType.output))}");
    builder.AppendLine($"  alt {string.Join(" ", arguments.FilterType(ValType.alt))}");
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
