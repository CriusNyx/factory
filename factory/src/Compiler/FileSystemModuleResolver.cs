namespace Factory.Compiler;

public class FileSystemModuleResolver(string rootPath) : ModuleResolver
{
  public string RootPath => rootPath;

  public bool TryReadFile(string path, out string source)
  {
    source = null!;
    try
    {
      if (File.Exists(path))
      {
        source = File.ReadAllText(path);
        return true;
      }
    }
    catch { }
    return false;
  }

  public ModuleResolution? Resolve(string modulePath)
  {
    var filePath = Path.Combine(rootPath, modulePath);
    if (TryReadFile(filePath, out var source))
    {
      var relativePath = Path.GetRelativePath(rootPath, filePath);
      return new ModuleResolution(relativePath, source);
    }
    return null;
  }
}
