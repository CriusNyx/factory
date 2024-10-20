﻿using GenParse.Functional;
using Factory.Parsing;
using System.CommandLine;

var options = CommandLineOptions.Create(args);

string GetProgramSourceCode()
{
  if (options.stream)
  {
    return Console.In.ReadToEnd();
  }
  else if (options.test)
  {
    return File.ReadAllText(
      Path.Join(AppDomain.CurrentDomain.BaseDirectory, "SamplePrograms/program1.factory")
    );
  }
  throw new CommandLineConfigurationException(
    "You must pass in an argument to specify where the source code should be read from."
  );
}

var source = GetProgramSourceCode();

var lexons = FactoryLexer.LexFactory(source);

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

var parser = FactoryParser.GenerateFactoryParser();
var ast = parser.Parse("Program", lexons.Filter(x => x.isSemantic))!;

if (options.ast)
{
  Console.WriteLine(ast.PrintProgram());
  return;
}

var context = new ExecutionContext();
var result = Executor.Evaluate(ast, ref context);
