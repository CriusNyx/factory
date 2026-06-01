using Factory;
using Factory.Compiler;
using static Factory.Superpower.ASTBuilder;

public class ModuleTests
{
  private static Dictionary<string, LanguageNode> CreateExpected(
    params (string moduleName, LanguageNode result)[] modules
  )
  {
    var output = new Dictionary<string, LanguageNode>();
    foreach (var element in modules)
    {
      output.Add(element.moduleName, element.result);
    }
    return output;
  }

  [Fact]
  public void CanResolveModuleWithNoImports()
  {
    var compiler = Compiler.CreateTestCompiler(("main.factory", ""));
    var actual = compiler.CompileRoot("main.factory");
    var expected = CreateExpected([("main.factory", Program())]);
    Assert.True(actual.Equivalent(expected));
  }

  [Fact]
  public void CanResolveModuleWithEmbeddedResource()
  {
    var compiler = Compiler.CreateTestCompiler(("main.factory", "import \"Satisfactory\""));
    var actual = compiler.CompileRoot("main.factory");
    var expected = CreateExpected(
      ("main.factory", Program(Import("Satisfactory"))),
      ("Satisfactory", EquivalentIfExists())
    );
    Assert.True(expected.DictionaryEquivalent(actual.Modules));
  }

  [Fact]
  public void CanResolveModuleWithRelativeImport()
  {
    var compiler = Compiler.CreateTestCompiler(
      ("main.factory", "import \"./other.factory\""),
      ("other.factory", "")
    );
    var actual = compiler.CompileRoot("main.factory");
    var expected = CreateExpected(
      ("main.factory", Program(Import("./other.factory"))),
      ("other.factory", Program())
    );
    Assert.True(expected.DictionaryEquivalent(actual.Modules));
  }

  [Fact]
  public void CanResolveImportType()
  {
    var compiler = Compiler.CreateTestCompiler(
      ("main.factory", "import \"./other.factory\""),
      ("other.factory", "let val = 0")
    );

    var program = compiler.CompileRoot("main.factory");
    var typeContext = new TypeContext(program, program.EntryModule);
    program.EntryModule.GetFactoryType(typeContext);
    var actual = typeContext.GetType("val");
    var expected = FactoryType.NumberType;
    Assert.Equal(actual, expected);
  }

  [Fact]
  public void CanResolveImportedValue()
  {
    var compiler = Compiler.CreateTestCompiler(
      ("main.factory", "import \"./other.factory\" print val"),
      ("other.factory", "let val = 1")
    );

    var program = compiler.CompileRoot("main.factory");
    var typeContext = new TypeContext(program, program.EntryModule);
    program.EntryModule.GetFactoryType(typeContext);

    StreamReader input = new StreamReader(new MemoryStream());
    StringWriter output = new StringWriter();

    // Evaluate
    var executionContext = new Factory.ExecutionContext(
      program,
      program.EntryModule,
      input,
      output
    );
    program.EntryModule.Evaluate(executionContext);

    Assert.Equal("1", output.ToString().TrimEnd());
  }
}
