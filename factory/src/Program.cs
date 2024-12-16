using Factory;
using GenParse.Functional;
using GenParse.Util;
using ExecutionContext = Factory.ExecutionContext;

bool debug = false;
#if DEBUG
debug = true;


#endif

#if !DEBUG
try
#endif
{
  var options = CommandLineOptions.Create(args);

  void EvaluateSourceCode(string sourceLocation, string sourceCode, string outFile = "")
  {
#if !DEBUG
    try
#endif
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

      var ast = FactoryParser.Parse(lexons)!;

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

      if (program == null)
      {
        throw new InvalidOperationException();
      }

      // Preform type validation.
      var typeContext = new TypeContext();
      program.CalculateType(typeContext);

      if (typeContext.Errors.Count() != 0)
      {
        foreach (var (position, length, message) in typeContext.Errors)
        {
          Console.WriteLine($"({position}, {length}): {message}".Colorize(CColor.Red));
        }
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
        File.WriteAllText(outFile, result);
      }
    }
#if !DEBUG
    catch (ParseException<FactoryLexon> e)
    {
      throw new FactoryParseException(sourceLocation, sourceCode, e.failedParseResult);
    }
#endif
  }

  if (options.debugGrammar)
  {
    var parser = FactoryParser.parser;
    foreach (var (setName, setValue) in parser.productionSets)
    {
      var name = setName;
      Console.WriteLine(name);
      foreach (var element in parser.ComputeHead(name))
      {
        Console.WriteLine($"  {element}");
      }
    }
  }
  else if (options.stream)
  {
    EvaluateSourceCode("stream", Console.In.ReadToEnd());
  }
  else if (options.file != "")
  {
    EvaluateSourceCode(options.file, File.ReadAllText(options.file), options.outFile);
  }
  else if (options.dir != "" || options.outDir != "")
  {
    if (options.dir == "")
    {
      throw new OutDirException();
    }
    var dir = options.dir;
    foreach (var file in Directory.GetFiles(dir))
    {
      var extension = Path.GetExtension(file);

      if (extension == ".factory" || extension == ".fact" || extension == ".fac")
      {
        if (options.outDir == "")
        {
          EvaluateSourceCode(file, File.ReadAllText(file));
        }
        else
        {
          if (!Directory.Exists(options.outDir))
          {
            Directory.CreateDirectory(options.outDir);
          }

          var outFile = Path.Combine(
            options.outDir,
            Path.GetFileNameWithoutExtension(file) + ".txt"
          );
          EvaluateSourceCode(file, File.ReadAllText(file), outFile);
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
#if !DEBUG
catch (Exception e)
{
  if (e is FactoryParseException parseException)
  {
    var lexon = parseException.failedParseResult.offendingLexon;
    var source =
      $"{parseException.Message}\n\n".Colorize(CColor.Red)
      + $"Failed to parse program from {parseException.sourceLocation}\n\n".Colorize(CColor.Red)
      + "---------------------------\n\n"
      + parseException.sourceCode.ReplaceAt(
        lexon?.index ?? 0,
        lexon?.length ?? 0,
        lexon?.sourceCode.Colorize(CColor.Red) ?? ""
      );
    Console.Error.WriteLine(source);
  }
  else
  {
    Console.Error.WriteLine(e.Message.Colorize(CColor.Red));
  }
}
#endif
