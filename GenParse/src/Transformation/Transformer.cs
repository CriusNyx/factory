using System.Reflection;
using GenParse.Functional;

public static class Transformer
{
  private static Dictionary<string, List<Type>> classCache = new Dictionary<string, List<Type>>();

  static Transformer()
  {
    foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()))
    {
      if (type.GetCustomAttribute<ASTClass>() is ASTClass aSTClass)
      {
        classCache.AddOrGet(aSTClass.nodeName).Add(type);
      }
    }
    foreach (var (key, value) in classCache)
    {
      Console.WriteLine(key);
      foreach (var type in value)
      {
        Console.WriteLine($"  {type}");
      }
    }
  }
}
