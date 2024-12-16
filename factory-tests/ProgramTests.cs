using Factory;
using GenParse.Functional;

namespace factory_tests
{
  public class ProgramTests
  {
    public const string inPath = "testFiles/in";
    public const string outPath = "testFiles/out";

    public static IEnumerable<object[]> EnumerateTestFiles()
    {
      return Directory
        .GetFiles(inPath)
        .Map(x => Path.GetFileNameWithoutExtension(x))
        .Map(x => new object[] { x });
    }

    [Theory]
    [MemberData(nameof(EnumerateTestFiles))]
    public void TestProgramsShouldProduceCorrectOutput(string filePath)
    {
      string inFile = Path.Combine(inPath, $"{filePath}.factory");
      string outFile = Path.Combine(outPath, $"{filePath}.txt");

      string sourceCode = File.ReadAllText(inFile);
      string expectedOutput = File.ReadAllText(outFile).ReplaceLineEndings("\n");
      string actualOutput = FactoryLanguage.Execute(sourceCode).ReplaceLineEndings("\n");

      Assert.Equal(expectedOutput, actualOutput);
    }
  }
}
