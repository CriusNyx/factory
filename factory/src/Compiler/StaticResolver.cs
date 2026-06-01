using Factory.Compiler;

public class StaticResolver(params (string path, string source)[] modules) : ModuleResolver
{
  public ModuleResolution? Resolve(string modulePath)
  {
    foreach (var mod in modules)
    {
      if (mod.path == modulePath)
      {
        return new ModuleResolution(mod.path, mod.source);
      }
    }

    return null;
  }
}
