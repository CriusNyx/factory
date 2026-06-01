namespace Factory.Compiler;

public interface ModuleResolver
{
  public ModuleResolution? Resolve(string modulePath);
}
