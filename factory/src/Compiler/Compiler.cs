using Factory.Util;

namespace Factory.Compiler;

public class Compiler
{
  static EmbeddedModuleResolver embeddedModuleResolver = new EmbeddedModuleResolver();
  ModuleResolver moduleResolver;

  public Compiler(ModuleResolver moduleResolver)
  {
    this.moduleResolver = moduleResolver;
  }

  public FactoryProgram CompileRoot(string entryPoint)
  {
    Dictionary<string, ModuleResolution?> resolutions = new Dictionary<string, ModuleResolution?>();
    Queue<ModuleResolution> modulesToParse = new Queue<ModuleResolution>();
    Dictionary<string, ProgramNode> parsedModules = new Dictionary<string, ProgramNode>();

    void LoadModule(string path)
    {
      var resolution = Resolve(path).NotNull($"Could not resolve module {path}");
      if (!resolutions.ContainsKey(resolution.UniqueModulePath))
      {
        resolutions.Add(resolution.UniqueModulePath, resolution);
        modulesToParse.Enqueue(resolution);
      }
    }

    var rootModule = moduleResolver
      .Resolve(entryPoint)
      .NotNull($"Could not resolve module {entryPoint}");
    resolutions.Add(rootModule.UniqueModulePath, rootModule);
    modulesToParse.Enqueue(rootModule);

    while (modulesToParse.TryDequeue(out var next))
    {
      var program = FactoryLanguage
        .Parse(next.SourceCode)
        .NotNull($"Could not parse module {rootModule}");
      program.FilePath = next.UniqueModulePath;
      parsedModules.Add(next.UniqueModulePath, program);
      foreach (var import in program.ResolveImports())
      {
        var importPath = ModuleResolution.GetModulePath(next.UniqueModulePath, import);
        LoadModule(importPath);
      }
    }

    return new FactoryProgram(entryPoint, parsedModules);
  }

  private ModuleResolution? Resolve(string path)
  {
    return embeddedModuleResolver.Resolve(path) ?? moduleResolver.Resolve(path);
  }

  public static Compiler CreateTestCompiler(params (string path, string source)[] modules)
  {
    return new Compiler(new StaticResolver(modules));
  }

  public static Compiler CreateFileCompiler(string rootPath)
  {
    return new Compiler(new FileSystemModuleResolver(rootPath));
  }
}
