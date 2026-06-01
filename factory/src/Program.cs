using Factory;
using Factory.Compiler;
#if !DEBUG
using Factory.Util;
#endif

bool debug = false;
#if DEBUG
debug = true;


#endif

#if !DEBUG
try
#endif
{
  var options = CommandLineOptions.Create(args);

  if (options.debugger)
  {
    Console.WriteLine("Waiting for debugger");
    while (!System.Diagnostics.Debugger.IsAttached)
    {
      Thread.Sleep(100);
    }
  }

  void EvaluateSourceCode(string sourceLocation, string sourceCode, string outFile = "")
  {
#if !DEBUG
    try
#endif
    {
      if (FactoryLanguage.TryExecute(sourceCode, out var result, options: options))
      {
        if (outFile != "")
        {
          File.WriteAllText(outFile, result);
        }
        else
        {
          Console.WriteLine(result);
        }
      }
      else
      {
        Console.WriteLine(result);
      }
    }
#if !DEBUG
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
#endif
  }

  if (options.script != null)
  {
    Scripts.RunScript(options);
    return;
  }

  if (!string.IsNullOrEmpty(options.profile))
  {
    Profiler.Profile(options.profile, 1000);
    return;
  }

  if (options.debugGrammar)
  {
    throw new NotImplementedException();
  }
  else if (options.satisfactorySource)
  {
    Console.Out.WriteLine(SourceGenerator.GenerateSatisfactorySource());
  }
  else if (options.stream)
  {
    FactoryLanguage.Execute(
      new StaticResolver(("main.factory", Console.In.ReadToEnd())),
      "main.factory",
      new StreamReader(new MemoryStream()),
      Console.Out
    );
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
    string debugPath = Path.GetFullPath("./SamplePrograms");
    FactoryLanguage.Execute(
      new FileSystemModuleResolver(debugPath),
      "debugProgram.factory",
      new StreamReader(new MemoryStream()),
      Console.Out
    );
  }
  else
  {
    throw new NoSourceFileException();
  }
}
#if !DEBUG
catch (Exception e)
{
  Console.WriteLine(e);
}
#endif
