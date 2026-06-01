using Factory.Compiler;

public class EmptyModuleResolver : ModuleResolver
{
  public ModuleResolution? Resolve(string modulePath)
  {
    return null;
  }
}
