using Factory.Compiler;
using Factory.Util;

namespace Factory;

public class FactoryProgram(string entryPoint, Dictionary<string, ProgramNode> modules)
{
  public string EntryPoint => entryPoint;
  public Dictionary<string, ProgramNode> Modules => modules;

  public bool Equivalent<T>(Dictionary<string, T> other)
    where T : LanguageNode
  {
    return modules.DictionaryEquivalent(other);
  }

  public ProgramNode? ResolveModule(string fromPath, string importPath)
  {
    return Modules.Safe(ModuleResolution.GetModulePath(fromPath, importPath));
  }

  public ProgramNode EntryModule => Modules[entryPoint];
}
