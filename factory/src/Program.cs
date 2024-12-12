using Factory.Parsing;
using GenParse.Functional;
using GenParse.Util;

bool debug = false;
#if DEBUG
debug = true;
#endif

try
{
  var options = CommandLineOptions.Create(args);

  var parser = FactoryParser.GenerateFactoryParser();

  void EvaluateSourceCode(string sourceLocation, string sourceCode, string outFile = "")
  {
    try
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
    catch (ParseException<FactoryLexon> e)
    {
      throw new FactoryParseException(sourceLocation, sourceCode, e.lexon);
    }
  }

  if (options.stream)
  {
    EvaluateSourceCode("stream", Console.In.ReadToEnd());
  }
  else if (options.file != "")
  {
    EvaluateSourceCode(File.ReadAllText(options.file), options.outOption);
  }
  else if (options.dirOption != "" || options.outDirOption != "")
  {
    if (options.dirOption == "")
    {
      throw new OutDirException();
    }
    var dir = options.dirOption;
    foreach (var file in Directory.GetFiles(dir))
    {
      var extension = Path.GetExtension(file);

      if (extension == ".factory" || extension == ".fact" || extension == ".fac")
      {
        if (options.outDirOption == "")
        {
          EvaluateSourceCode(file, File.ReadAllText(file));
        }
        else
        {
          if (!Directory.Exists(options.outDirOption))
          {
            Directory.CreateDirectory(options.outDirOption);
          }

          var outFile = Path.Combine(
            options.outDirOption,
            Path.GetFileNameWithoutExtension(file) + ".txt"
          );
          EvaluateSourceCode(File.ReadAllText(file), outFile);
        }
      }
    }
  }
  else if (debug)
  {
    string debugFile = "./SamplePrograms/program1.factory";
    EvaluateSourceCode(debugFile, File.ReadAllText(debugFile));
  }
  else
  {
    throw new NoSourceFileException();
  }
}
catch (Exception e)
{
  if (e is FactoryParseException parseException)
  {
    var lexon = parseException.lexon;
    var source =
      $"Unexpected symbol {parseException.lexon.sourceCode}\n\n".Colorize(CColor.Red)
      + $"Failed to parse program from {parseException.sourceLocation}\n\n".Colorize(CColor.Red)
      + "---------------------------\n\n"
      + parseException.sourceCode.ReplaceAt(
        lexon.index,
        lexon.length,
        lexon.sourceCode.Colorize(CColor.Red)
      );
    Console.Error.WriteLine(source);
  }
  else
  {
    Console.Error.WriteLine(e.Message.Colorize(CColor.Red));
  }
}
