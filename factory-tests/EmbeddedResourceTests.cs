using Factory.Compiler;

public class EmbeddedResourceChecks
{
  public static IEnumerable<object[]> EnumerateTestFiles()
  {
    yield return ["Satisfactory"];
  }

  [Theory]
  [InlineData("Satisfactory")]
  public void EmbeddedResourceExists(string moduleName)
  {
    var embeddedModuleResolver = new EmbeddedModuleResolver();
    Assert.NotNull(embeddedModuleResolver.Resolve(moduleName));
  }
}
