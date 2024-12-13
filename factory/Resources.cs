using System.Reflection;

public static class Resources
{
  public static string GetPathForResource(string localPath)
  {
    string assemPath = Directory.GetParent(Assembly.GetCallingAssembly().Location)!.ToString();
    return Path.Join(assemPath, localPath);
  }
}
