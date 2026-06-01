using System.Reflection;
using Factory.Util;

namespace Factory.Compiler;

public class EmbeddedModuleResolver : ModuleResolver
{
  const string EmbeddedResourceNamespace = "factory.src.EmbeddedModules";

  public ModuleResolution? Resolve(string modulePath)
  {
    string resourceNamespace = $"{EmbeddedResourceNamespace}.{modulePath}.factory";
    try
    {
      var source = Assembly
        .GetAssembly(typeof(FactoryLanguage))
        .NotNull()
        .GetManifestResourceStream(resourceNamespace)
        .SafePipe(x => new StreamReader(x).ReadToEnd());
      if (source != null)
      {
        return new ModuleResolution(modulePath, source);
      }
    }
    catch { }
    return null;
  }
}
