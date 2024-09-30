using GenParse.Functional;
using Newtonsoft.Json;

[Serializable]
public class Docs
{
  public readonly static Docs docs;
  public readonly static Dictionary<string, Item> itemsByClass;
  public readonly static Dictionary<string, Item> itemsByIdentifier;
  public readonly static Dictionary<string, Recipe[]> recipesByProductClass;
  public readonly static Dictionary<string, Recipe[]> recipesByProductIdentifier;

  static Docs()
  {
    var baseDir = AppDomain.CurrentDomain.BaseDirectory;

    var docsJson = File.ReadAllText(Path.Join(baseDir, "Docs.json"));

    docs = JsonConvert.DeserializeObject<Docs>(docsJson)!;
    itemsByClass = docs.Desc.ToDictionary(x => x.className);
    itemsByIdentifier = docs.Desc
      .Filter(x => (x.identifier ?? "") != "")
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
      if (!_recipesByProductClass.ContainsKey(primaryProductClass))
      {
        _recipesByProductClass.Add(primaryProductClass, new List<Recipe>());
      }
      _recipesByProductClass[primaryProductClass].Add(recipe);

      var productIdent = itemsByClass.Safe(primaryProductClass)?.identifier;
      if (productIdent == null)
      {
        continue;
      }
      if (!_recipesByProductIdentifier.ContainsKey(productIdent))
      {
        _recipesByProductIdentifier.Add(productIdent, new List<Recipe>());
      }
      _recipesByProductIdentifier[productIdent].Add(recipe);
    }
    recipesByProductClass = _recipesByProductClass.ToDictionary(x => x.Key, y => y.Value.ToArray());
    recipesByProductIdentifier = _recipesByProductIdentifier.ToDictionary(
      x => x.Key,
      y => y.Value.OrderBy(x => x.isMachineRecipe && !x.isAlternative ? 0 : 1).ToArray()
    );
  }

  public Item[] Desc;
  public Recipe[] Recipe;

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
    "Build_FoundryMk1_C"
  };
}
