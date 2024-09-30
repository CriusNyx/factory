using System.CommandLine;

public class CommandLineOptions
{
  public readonly bool stream;
  public readonly bool test;

  public CommandLineOptions(bool stream, bool test)
  {
    this.stream = stream;
    this.test = true;
  }

  public static CommandLineOptions Create(params string[] args)
  {
    var streamOption = new Option<bool>("--stream");
    var testOption = new Option<bool>("--test");

    var rootCommand = new RootCommand().Option(streamOption).Option(testOption);

    CommandLineOptions output = null!;

    void Construct(bool stream, bool test)
    {
      output = new CommandLineOptions(stream, test);
    }

    rootCommand.SetHandler(Construct, streamOption, testOption);

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
