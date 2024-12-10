using System.CommandLine;

public class CommandLineOptions
{
  public readonly string file;
  public readonly string outOption;
  public readonly string dirOption;
  public readonly string outDirOption;
  public readonly bool stream;
  public readonly bool ast;
  public readonly bool test;
  public readonly bool lexons;
  public readonly bool transform;

  public CommandLineOptions(
    string file,
    string outOption,
    string dirOption,
    string outDirOption,
    bool stream,
    bool ast,
    bool lexons,
    bool transform
  )
  {
    this.file = file;
    this.outOption = outOption;
    this.dirOption = dirOption;
    this.outDirOption = outDirOption;
    this.stream = stream;
    this.ast = ast;
    this.lexons = lexons;
    this.transform = transform;
  }

  public static CommandLineOptions Create(params string[] args)
  {
    var fileArg = new Argument<string>("file");
    fileArg.SetDefaultValue("");

    var outOption = new Option<string>("--out");
    outOption.SetDefaultValue("");

    var dirOption = new Option<string>("--dir");
    dirOption.SetDefaultValue("");

    var outDirOption = new Option<string>("--outdir");
    outDirOption.SetDefaultValue("");

    var streamOption = new Option<bool>("--stream");
    var astOption = new Option<bool>("--ast");
    var lexonOption = new Option<bool>("--lexons");
    var transformOption = new Option<bool>("--transform");

    var rootCommand = new RootCommand()
      .Argument(fileArg)
      .Option(outOption)
      .Option(dirOption)
      .Option(outDirOption)
      .Option(streamOption)
      .Option(astOption)
      .Option(lexonOption)
      .Option(transformOption);

    CommandLineOptions output = null!;

    void Construct(
      string fileArgument,
      string outOption,
      string dirOption,
      string outDirOption,
      bool stream,
      bool ast,
      bool lexons,
      bool transform
    )
    {
      output = new CommandLineOptions(
        fileArgument,
        outOption,
        dirOption,
        outDirOption,
        stream,
        ast,
        lexons,
        transform
      );
    }

    rootCommand.SetHandler(
      Construct,
      fileArg,
      outOption,
      dirOption,
      outDirOption,
      streamOption,
      astOption,
      lexonOption,
      transformOption
    );

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

  public static RootCommand Argument(this RootCommand rootCommand, Argument argument)
  {
    rootCommand.AddArgument(argument);
    return rootCommand;
  }
}
