using CommandLine;
using Parser = CommandLine.Parser;

namespace Factory;

public class CommandLineOptions
{
  [Value(0, Default = "", HelpText = "The file to parse")]
  public string file { get; set; }

  [Option('o', "out", Default = "", HelpText = "File to write output to.")]
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

  [Option("types", HelpText = "Print program types")]
  public bool types { get; set; }

  [Option('g', "debug-grammar", HelpText = "Print debug information for program grammar")]
  public bool debugGrammar { get; set; }

  [Option(
    "debugger",
    HelpText = "Wait for debugger to attach before running the rest of the program"
  )]
  public bool debugger { get; set; }

  [Option("profile", HelpText = "Run requested profile method")]
  public string profile { get; set; }

  [Option("script", HelpText = "Run a named script")]
  public string script { get; set; }

  public static CommandLineOptions Create(params string[] args)
  {
    CommandLineOptions output = null!;
    Parser
      .Default.ParseArguments<CommandLineOptions>(args)
      .WithParsed(
        (options) =>
        {
          output = options;
        }
      );
    return output;
  }

  public static CommandLineOptions Default => Create([]);

  public override string ToString()
  {
    return $"stream = {stream}";
  }
}
