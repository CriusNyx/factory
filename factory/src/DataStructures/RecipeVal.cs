using Factory;

public class RecipeVal(string name, bool isAlternative, Ratio ratio) : HasRecipe
{
  public string Name => name;
  public bool IsAlternative => isAlternative;
  public Ratio Ratio => ratio;

  public RecipeVal Recipe => this;

  public override string ToString()
  {
    return $"{Name}: {(IsAlternative ? "alt " : "")}{Ratio}";
  }
}
