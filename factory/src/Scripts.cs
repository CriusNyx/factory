using System;
using System.Linq;
using System.Reflection;
using Factory;
using SharpParse.Functional;
using SharpParse.Grammar;

public class Scripts
{
  public static void RunScript(CommandLineOptions commandLineOptions)
  {
    var scriptName = commandLineOptions.script;

    // Change into Pascal case
    var pascalName = char.ToUpper(scriptName[0]) + scriptName.Substring(1);

    // Catch edge case where RunScript is called recursively.
    if (pascalName == "RunScript")
    {
      throw new NotSupportedException(scriptName);
    }

    var method = typeof(Scripts).GetMethod(pascalName);

    if (method == null)
    {
      throw new NotSupportedException(scriptName);
    }

    InvokeScript(method, commandLineOptions, scriptName);
  }

  private static void InvokeScript(
    MethodInfo methodInfo,
    CommandLineOptions commandLineOptions,
    string scriptName
  )
  {
    var paramTypes = methodInfo.GetParameters().Map(x => x.ParameterType);

    if (paramTypes.SequenceEqual([]))
    {
      methodInfo.Invoke(null, []);
    }
    else if (paramTypes.SequenceEqual([typeof(CommandLineOptions)]))
    {
      methodInfo.Invoke(null, [commandLineOptions]);
    }
    else
    {
      throw new NotSupportedException(scriptName);
    }
  }

  public static void TestGrammar()
  {
    var grammarFilePath = "Grammar/factory.grammar";
    var grammarFileText = File.ReadAllText(grammarFilePath);
    var grammar = GrammarParser.TestParse(grammarFileText);
    Console.WriteLine(grammar);
  }
}

public class NoScriptException(string scriptName) : Exception($"Unknown script {scriptName}") { }
