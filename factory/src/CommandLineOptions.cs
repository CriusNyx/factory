using CommandLine;

public class CommandLineOptions
{
  [Value(0, Default = "", HelpText = "The file to parse")]
  public string file { get; set; }

  [Option('o', "outDir", Default = "", HelpText = "File to write output to.")]
  public string outFile { get; set; }

  [Option('d', "dir", Default = "", HelpText = "Directory to read input from.")]
  public string dir { get; set; }

  [Option("outdir", Default = "", HelpText = "Directory to write output to.")]
  public string outDir { get; set; }

  [Option('s', "stream", HelpText = "Stream input from stdio")]
  public bool stream { get; set; }

  [Option('a', "ast", HelpText = "Print the abstract syntax tree")]
  public bool ast { get; set; }

  [Option("test", HelpText = "Test the program using the test source code")]
  public bool test { get; set; }

  [Option('l', "lexons", HelpText = "Print the lexons")]
  public bool lexons { get; set; }

  [Option('t', "transform", HelpText = "Print the AST after transformation")]
  public bool transform { get; set; }

  [Option('g', "debug-grammar", HelpText = "Print debug information for program grammar")]
  public bool debugGrammar { get; set; }

  public static CommandLineOptions Create(params string[] args)
  {
    CommandLineOptions output = null!;
    CommandLine
      .Parser.Default.ParseArguments<CommandLineOptions>(args)
      .WithParsed(
        (options) =>
        {
          output = options;
        }
      );
    return output;
  }

  public override string ToString()
  {
    return $"stream = {stream}";
  }
}
