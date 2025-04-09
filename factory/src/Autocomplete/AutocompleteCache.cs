using System.Reflection;
using Factory;
using SharpParse.Functional;
using SharpParse.Util;

public static class AutocompleteCache
{
  private static Dictionary<string, List<(RefInfo?, FactoryType)>> autocompleteStringTypes =
    new Dictionary<string, List<(RefInfo?, FactoryType)>>();

  private static AutocompleteSourceNode root;

  public static (RefInfo? refInfo, FactoryType factoryType)[] TypesForString(string str) =>
    autocompleteStringTypes.Safe(str)?.ToArray() ?? [];

  static AutocompleteCache()
  {
    var recipesStrings = GetRecipeStrings();
    var memberInfo = GetExposedMembers();
    var searchStrings = recipesStrings.Concat(memberInfo.Map(x => x.name)).Distinct().ToArray();
    root = BuildAutocomplete(
      recipesStrings.Concat(memberInfo.Map(x => x.name)).Distinct().ToArray()
    );

    foreach (var recipeName in recipesStrings)
    {
      autocompleteStringTypes
        .AddOrGet(recipeName)
        .Add((null, FactoryType.FromCSharpType(typeof(Recipe))));
    }
    foreach (var (name, refInfo, factoryType) in memberInfo)
    {
      autocompleteStringTypes.AddOrGet(name).Add((refInfo, factoryType));
    }

    List<string[]> autocompleteEntries = new List<string[]>();
    foreach (var list in autocompleteStringTypes.Values)
    {
      foreach (var (refInfo, type) in list)
      {
        autocompleteEntries.Add([refInfo?.ToShortString() ?? "", type.ToShortString()]);
      }
    }

    Console.WriteLine(Formatting.PrintGrid(autocompleteEntries.ToArray()));
  }

  private static string[] GetRecipeStrings()
  {
    return Docs
      .recipesByIdentifier.Keys.Concat(Docs.recipesByProductIdentifier.Keys)
      .Distinct()
      .ToArray();
  }

  private static (string name, RefInfo refInfo, FactoryType factoryType)[] GetExposedMembers()
  {
    return typeof(AutocompleteCache)
      .Assembly.GetTypes()
      .FlatMap(type => type.GetMembers())
      .Map(member => (member, attr: member.GetCustomAttribute<ExposeMemberAttribute>()))
      .Filter(pair => pair.attr != null)
      .Map(pair =>
        (
          pair.attr!.name,
          new RefInfo(pair.member.DeclaringType?.Name!, pair.attr.name),
          FactoryType.FromCSharpMember(pair.member)
        )
      );
  }

  public static string[] Search(string searchString)
  {
    return root.Search(searchString).EnumerateCompleteStrings().ToArray();
  }

  private static AutocompleteSourceNode BuildAutocomplete(string[] searchStrings)
  {
    var root = new AutocompleteSourceNode(null, '\0');
    foreach (var searchString in searchStrings)
    {
      root.BuildPath(searchString);
    }
    root.Sort();
    return root;
  }

  public static string[] PrintSearchStrings()
  {
    return root.EnumerateCompleteStrings().ToArray();
  }

  public static string[] PrintSearchTree()
  {
    return root.EnumerateStrings().ToArray();
  }
}

class AutocompleteSourceNode(AutocompleteSourceNode? parent, char character)
{
  AutocompleteSourceNode? parent = parent;
  char character = character;
  public bool isResult = false;
  List<AutocompleteSourceNode> children = new List<AutocompleteSourceNode>();

  public AutocompleteSourceNode Search(string searchString)
  {
    if (searchString == "")
    {
      return this;
    }
    foreach (var child in children)
    {
      if (child.character == searchString[0])
      {
        return child.Search(searchString.Substring(1));
      }
    }
    return this;
  }

  public void BuildPath(string searchString)
  {
    if (searchString == "")
    {
      isResult = true;
      return;
    }
    children
      .FindOrAdd(
        x => char.ToLower(x.character) == char.ToLower(searchString[0]),
        () => new AutocompleteSourceNode(this, searchString[0])
      )
      .BuildPath(searchString.Substring(1));
  }

  public IEnumerable<string> EnumerateStrings()
  {
    if (this.character != '\0')
    {
      yield return $"{this.character}";
    }
    foreach (var child in children)
    {
      foreach (var str in child.EnumerateStrings())
      {
        if (this.character != '\0')
        {
          yield return this.character + str;
        }
        else
        {
          yield return str;
        }
      }
    }
  }

  public string ParentString()
  {
    if (parent == null)
    {
      return "";
    }
    return parent.ParentString() + character;
  }

  public IEnumerable<string> EnumerateCompleteStrings()
  {
    if (isResult)
    {
      yield return $"{parent?.ParentString()}{this.character}";
    }
    else
    {
      foreach (var child in children)
      {
        foreach (var str in child.EnumerateCompleteStrings())
        {
          yield return str;
        }
      }
    }
  }

  public void Sort()
  {
    children = children.OrderBy(x => x.character).ToList();
    foreach (var child in children)
    {
      child.Sort();
    }
  }
}
