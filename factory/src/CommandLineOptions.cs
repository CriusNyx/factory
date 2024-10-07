using System.CommandLine;

public class CommandLineOptions
{
  public readonly bool stream;
  public readonly bool ast;
  public readonly bool test;
  public readonly bool lexons;

  public CommandLineOptions(bool stream, bool test, bool ast, bool lexons)
  {
    this.stream = stream;
#if DEBUG
    this.test = true;
#else
    this.test = test;
#endif
    this.ast = ast;
    this.lexons = lexons;
  }

  public static CommandLineOptions Create(params string[] args)
  {
    var streamOption = new Option<bool>("--stream");
    var testOption = new Option<bool>("--test");
    var astOption = new Option<bool>("--ast");
    var lexonOption = new Option<bool>("--lexons");

    var rootCommand = new RootCommand()
      .Option(streamOption)
      .Option(testOption)
      .Option(astOption)
      .Option(lexonOption);

    CommandLineOptions output = null!;

    void Construct(bool stream, bool test, bool ast, bool lexons)
    {
      output = new CommandLineOptions(stream, test, ast, lexons);
    }

    rootCommand.SetHandler(Construct, streamOption, testOption, astOption, lexonOption);

    rootCommand.Invoke(args);

    return output;
  }

  public override string ToString()
  {
    return $"stream = {stream}";
  }
}

public static class CommandLineExtensions
{
  public static RootCommand Option(this RootCommand rootCommand, Option option)
  {
    rootCommand.AddOption(option);
    return rootCommand;
  }
}
