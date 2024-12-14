using GenParse.Functional;
using Newtonsoft.Json;

namespace Factory;

[Serializable]
public class Docs
{
  public Item[] Desc;
  public Recipe[] Recipe;

  public static readonly Docs docs;
  public static readonly Dictionary<string, Item> itemsByClass;
  public static readonly Dictionary<string, Item> itemsByIdentifier;
  public static readonly Dictionary<string, Recipe[]> recipesByProductClass;
  public static readonly Dictionary<string, Recipe[]> recipesByProductIdentifier;

  static Docs()
  {
    try
    {
      var docsJson = File.ReadAllText(Resources.GetPathForResource("docs.json"));

      docs = JsonConvert.DeserializeObject<Docs>(docsJson)!;
      itemsByClass = docs.Desc.ToDictionary(x => x.className);
      itemsByIdentifier = docs
        .Desc.Filter(x => (x.identifier ?? "") != "")
        .ToDictionary(x => x.identifier);

      Dictionary<string, List<Recipe>> _recipesByProductClass =
        new Dictionary<string, List<Recipe>>();
      Dictionary<string, List<Recipe>> _recipesByProductIdentifier =
        new Dictionary<string, List<Recipe>>();

      foreach (var recipe in docs.Recipe)
      {
        var primaryProductClass = recipe.primaryProductClass;
        if (primaryProductClass == null)
        {
          continue;
        }

        _recipesByProductClass.AddOrGet(primaryProductClass).Add(recipe);

        var productIdent = itemsByClass.Safe(primaryProductClass)?.identifier;
        if (productIdent == null)
        {
          continue;
        }
        _recipesByProductIdentifier.AddOrGet(productIdent).Add(recipe);
      }

      recipesByProductClass = _recipesByProductClass.ToDictionary(
        x => x.Key,
        y => y.Value.ToArray()
      );
      recipesByProductIdentifier = _recipesByProductIdentifier.ToDictionary(
        x => x.Key,
        y => y.Value.OrderBy(x => x.isMachineRecipe && !x.isAlternative ? 0 : 1).ToArray()
      );
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
  }

  public static readonly string[] ProductionMachines = new string[]
  {
    "Build_ConstructorMk1_C",
    "Build_SmelterMk1_C",
    "Build_Blender_C",
    "Build_Packager_C",
    "Build_Converter_C",
    "Build_HadronCollider_C",
    "Build_QuantumEncoder_C",
    "Build_OilRefinery_C",
    "Build_ManufacturerMk1_C",
    "Build_AssemblerMk1_C",
    "Build_FoundryMk1_C",
  };
}
