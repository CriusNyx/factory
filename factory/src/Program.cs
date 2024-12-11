using Factory.Parsing;
using GenParse.Functional;

bool debug = false;
#if DEBUG
debug = true;
#endif

var options = CommandLineOptions.Create(args);

var parser = FactoryParser.GenerateFactoryParser();

void EvaluateSourceCode(string sourceCode, string outFile = "")
{
  var lexons = FactoryLexer.LexFactory(sourceCode);

  if (options.lexons)
  {
    var lexonsString = string.Join(
      "\n",
      lexons
        .Filter(x => x.isSemantic)
        .Map(x => $"{x.lexonType.ToString().PadRight(20)} {x.sourceCode}")
    );
    Console.WriteLine(lexonsString);
    return;
  }

  var ast = parser.Parse("Program", lexons.Filter(x => x.isSemantic))!;

  if (options.ast)
  {
    Console.WriteLine(ast.PrintProgram());
  }

  var program = Transformer.Transform(ast) as ProgramNode;

  if (options.transform)
  {
    Console.WriteLine(program!.ToTree());
  }

  if (options.ast || options.transform)
  {
    return;
  }

  using var textWriter = new StringWriter();
  using var context = new ExecutionContext(Console.In, textWriter);

  program!.Evaluate(context);

  var result = textWriter.ToString().TrimEnd();

  if (outFile == "")
  {
    Console.WriteLine(result);
  }
  else
  {
    File.WriteAllText(outFile, result + Environment.NewLine);
  }
}

if (options.stream)
{
  EvaluateSourceCode(Console.In.ReadToEnd());
}
else if (options.file != "")
{
  EvaluateSourceCode(File.ReadAllText(options.file), options.outOption);
}
else if (options.dirOption != "" || options.outDirOption != "")
{
  if (options.dirOption == "")
  {
    Console.Error.WriteLine(
      "The --dir command line option must be specified to use the --outDir command line option."
    );
    return;
  }
  var dir = options.dirOption;
  foreach (var file in Directory.GetFiles(dir))
  {
    var extension = Path.GetExtension(file);

    if (extension == ".factory" || extension == ".fact" || extension == ".fac")
    {
      if (options.outDirOption == "")
      {
        EvaluateSourceCode(File.ReadAllText(file));
      }
      else
      {
        if (!Directory.Exists(options.outDirOption))
        {
          Directory.CreateDirectory(options.outDirOption);
        }

        var outFile = Path.Combine(options.outDirOption, Path.GetFileName(file));
        EvaluateSourceCode(File.ReadAllText(file), outFile);
      }
    }
  }
}
else if (debug)
{
  EvaluateSourceCode(File.ReadAllText("./SamplePrograms/program1.factory"));
}
else
{
  Console.Error.WriteLine("No source file specified.");
}
