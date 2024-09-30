public class RecipeValue
{
  public readonly string recipeName;
  public readonly string[] inputs;
  public readonly string[] outputs;
  public readonly string[] alts;

  public RecipeValue(string recipeName, string[] inputs, string[] outputs, string[] alts)
  {
    this.recipeName = recipeName;
    this.inputs = inputs;
    this.outputs = outputs;
    this.alts = alts;
  }

  public override string ToString()
  {
    return $"recipe {recipeName}\n  in {string.Join(" ", inputs)}\n  out {string.Join(" ", outputs)}\n  alt {string.Join(" ", alts)}";
  }
}
