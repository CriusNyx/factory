namespace Factory.Compiler;

public class ModuleResolution(string uniqueModulePath, string sourceCode)
{
  public string UniqueModulePath => uniqueModulePath;
  public string SourceCode => sourceCode;

  public static string GetModulePath(string fromModulePath, string importPath)
  {
    if (importPath.StartsWith("./"))
    {
      return Path.GetRelativePath(".", Path.Join(fromModulePath, "..", importPath));
    }
    return importPath;
  }
}
