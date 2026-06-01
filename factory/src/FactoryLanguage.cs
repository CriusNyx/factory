using System.Collections;
using System.Diagnostics;
using System.Text;
using Factory.Compiler;
using Factory.Superpower;
using Factory.Util;
using Newtonsoft.Json;
using Superpower;
using Superpower.Model;

namespace Factory;

public class LanguageDiagnostics : IEnumerable<string>
{
  private Stopwatch stopwatch = new Stopwatch();
  private List<string> diagnostics = new List<string>();

  public void Start() => stopwatch.Start();

  public void Reset()
  {
    stopwatch.Reset();
  }

  public void LapAndLog(Func<long, string> func)
  {
    diagnostics.Add(func(stopwatch.Lap()));
  }

  public void Add(string message) => diagnostics.Add(message);

  public static LanguageDiagnostics CreateAndStart()
  {
    return new LanguageDiagnostics().Touch(x => x.Start());
  }

  public IEnumerator<string> GetEnumerator()
  {
    return diagnostics.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return diagnostics.GetEnumerator();
  }
}

/// <summary>
/// Class to process factory language source code and produce results.
/// </summary>
public static class FactoryLanguage
{
  public class AnalyzeSemanticsResults(FactorySemanticToken[] output, string[] diagnostics)
  {
    public FactorySemanticToken[] Output => output;
    public string[] Diagnostics => diagnostics;

    public string ToJson()
    {
      return JsonConvert.SerializeObject(this);
    }
  }

  /// <summary>
  /// Analyze the semantic content of source code for Factory LSP.
  /// </summary>
  /// <param name="sourceCode"></param>
  /// <returns></returns>
  public static AnalyzeSemanticsResults AnalyzeSemanticTokens(string sourceCode)
  {
    var diagnostics = LanguageDiagnostics.CreateAndStart();

    var tokenList = SuperpowerTokenizer.Tokenize(sourceCode, false);
    diagnostics.LapAndLog(x => $"Tokenizer time {x}");

    var info = SuperpowerParser
      .ShallowParser.Parse(
        new TokenList<SuperpowerTokenType>(tokenList.Where(x => x.IsSemantic()).ToArray())
      )
      .FilterDefined();
    var tokens = tokenList.ToArray();

    var output = tokens
      .Map(x => (lexon: x, semanticType: x.GetSemanticType()))
      .Filter(x => x.semanticType != FactorySemanticType.whitespace)
      .Map(x => new FactorySemanticToken(
        x.lexon.Position.Absolute,
        x.lexon.Span.Length,
        x.semanticType,
        (int)FactorySemanticModifier.none
      ));

    HashSet<string> readonlySymbols = new HashSet<string>();

    foreach (var i in info)
    {
      if (i.IsReadonly)
      {
        Console.WriteLine($"readonly source: {i.InfoPosition} \"{i.Source}\"");
        readonlySymbols.Add(i.Source);
      }
    }

    output = output.Map(token =>
    {
      string source = sourceCode.Substring(token.position, token.length);
      Console.WriteLine($"token source: \"{source}\"");
      if (readonlySymbols.Contains(source))
      {
        Console.WriteLine("Marking Readonly");
        token.modifier = (int)FactorySemanticModifier.@readonly;
      }
      return token;
    });

    return new AnalyzeSemanticsResults(output, diagnostics.ToArray());
  }

  /// <summary>
  /// Get a list of supported semantic types for the factory language.
  /// </summary>
  /// <returns></returns>
  public static string[] GetSemanticTypes()
  {
    return Enum.GetValues<FactorySemanticType>().Map(x => x.ToString());
  }

  /// <summary>
  /// Get a list of supported semantic modifier for the factory language.
  /// </summary>
  /// <returns></returns>
  public static string[] GetSemanticModifiers()
  {
    // The number of valid flags is 1 less then the number of modifiers because None is not a flag.
    int count = Enum.GetValues<FactorySemanticModifier>().Length - 1;
    return Functional.Range(count).Map(x => ((FactorySemanticModifier)(1 << x)).ToString());
  }

  public class HoverResult(string? _hoverString, string[] _diagnostics)
  {
    public string? hoverString => _hoverString;
    public string[] diagnostics => _diagnostics;
  }

  public static HoverResult GetHoverInfo(string sourceCode, int hoverIndex)
  {
    LanguageDiagnostics diagnostics = LanguageDiagnostics.CreateAndStart();
    try
    {
      var (program, _) = TypeCheck(sourceCode);
      diagnostics.LapAndLog(x => $"Type Check Time {x}");

      if (program == null)
      {
        return new HoverResult("", diagnostics.ToArray());
      }

      var hoverString = program.GetHoverString(hoverIndex);
      diagnostics.LapAndLog(x => $"Hover String Time {x}");
      return new HoverResult(hoverString, diagnostics.ToArray());
    }
    catch (Exception e)
    {
      diagnostics.Add(e.ToString());
      return new HoverResult(null, diagnostics.ToArray());
    }
  }

  public static (string label, string documentation)[] GetAutocompleteStrings(
    string sourceCode,
    int index
  )
  {
    Token<SuperpowerTokenType>?[] tokens = SuperpowerTokenizer
      .Tokenize(sourceCode)
      .Select(x => x as Token<SuperpowerTokenType>?)
      .ToArray();
    var owner = tokens.FirstOrDefault(x =>
      x!.Value.HasIndex(index) && x.Value.Kind == SuperpowerTokenType.symbol
    );
    if (owner == null)
    {
      return [];
    }
    var output = AutocompleteCache
      .Search(owner.Value.ToStringValue())
      .Concat(
        tokens
          .Filter(x => x!.Value.Kind == SuperpowerTokenType.symbol)
          .Map(x => x!.Value.ToStringValue())
          .Where(x => x.StartsWith(owner.Value.ToStringValue()))
      )
      .Distinct()
      .ToArray();

    Array.Sort(output);

    return output.Map(x => x.With(GetDocumentation(x)));
  }

  private static string GetDocumentation(string label)
  {
    List<string> output = new List<string>();
    foreach (var (refInfo, type) in AutocompleteCache.TypesForString(label))
    {
      if (type is MethodType methodType)
      {
        output.Add(methodType.ToShortString());
      }
      else
      {
        output.Add(
          string.Join(
            ": ",
            Functional.FromDefined(refInfo?.ToShortString() ?? label, type.ToShortString())
          )
        );
      }
    }
    return string.Join("\n\n", output);
  }

  public class AnalyzeErrorsResult(FactoryLanguageError[] errors, string[] diagnostics)
  {
    public FactoryLanguageError[] Errors => errors;
    public string[] Diagnostics => diagnostics;
  }

  /// <summary>
  /// Analyze the factory language for errors.
  /// </summary>
  /// <param name="sourceCode"></param>
  /// <returns></returns>
  public static AnalyzeErrorsResult AnalyzeErrors(string sourceCode)
  {
    var diagnostics = new LanguageDiagnostics();

    // Initialize output.
    List<FactoryLanguageError> errors = new List<FactoryLanguageError>();

    try
    {
      Token<SuperpowerTokenType>[] lexons = SuperpowerTokenizer.Tokenize(sourceCode).ToArray();
      diagnostics.LapAndLog((x) => $"Tokenizing Time {x}");

      // Crawl lexons and check for errors
      for (int i = 0; i < lexons.Length; i++)
      {
        var lexon = lexons[i];
        if (lexon.Kind == SuperpowerTokenType.unknown)
        {
          errors.Add(
            new FactoryLanguageError(
              lexon.Position.Absolute,
              lexon.Span.Length,
              FactoryErrorType.error,
              "Unknown lexon"
            )
          );
        }
      }

      diagnostics.LapAndLog((x) => $"Token Error Time {x}");

      // Parse factory language
      var result = SuperpowerParser.TryParse(sourceCode);

      diagnostics.LapAndLog((x) => $"Parse Time {x}");

      if (!result.HasValue)
      {
        errors.Add(
          new FactoryLanguageError(
            result.ErrorPosition.Absolute,
            1,
            FactoryErrorType.error,
            result.FormatErrorMessageFragment()
          )
        );
        diagnostics.LapAndLog((x) => $"Parse Error Time {x}");
      }
      else
      {
        var typeContext = new TypeContext();
        var program = result.Value;

        program?.GetFactoryType(typeContext);
        diagnostics.LapAndLog((x) => $"Type Check Time {x}");
        foreach (var error in typeContext.Errors)
        {
          errors.Add(
            new FactoryLanguageError(
              error.position,
              error.length,
              FactoryErrorType.error,
              error.message
            )
          );
        }

        diagnostics.LapAndLog((x) => $"Type Error Time {x}");
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
    return new AnalyzeErrorsResult(errors.ToArray(), diagnostics.ToArray());
  }

  /// <summary>
  /// Resolve language globals.
  /// </summary>
  /// <param name="symbol"></param>
  /// <returns></returns>
  public static object? ResolveGlobal(string symbol)
  {
    return Docs.recipesByProductIdentifier.Safe(symbol)?.FirstOrDefault()
      ?? Docs.recipesByIdentifier.Safe(symbol)?.FirstOrDefault();
  }

  /// <summary>
  /// Parse the factory language.
  /// </summary>
  /// <param name="sourceCode"></param>
  /// <returns></returns>
  public static ProgramNode Parse(string sourceCode)
  {
    return SuperpowerParser.ParseString(sourceCode, SuperpowerParser.ProgramParser);
  }

  /// <summary>
  /// Type check the program.
  /// </summary>
  /// <param name="program"></param>
  /// <param name="program"></param>
  /// <returns></returns>
  public static (ProgramNode program, TypeContext typeContext) TypeCheck(ProgramNode program)
  {
    var typeContext = new TypeContext();
    program.GetFactoryType(typeContext);
    program.RuntimeCheckTypeValidity();
    return (program, typeContext);
  }

  /// <summary>
  /// Type check the source code.
  /// </summary>
  /// <param name="program"></param>
  /// <param name="sourceCode"></param>
  /// <returns></returns>
  public static (ProgramNode program, TypeContext typeContext) TypeCheck(string sourceCode)
  {
    return TypeCheck(Parse(sourceCode));
  }

  /// <summary>
  /// Completely compile the program source code.
  /// </summary>
  /// <param name="sourceCode"></param>
  /// <returns></returns>
  public static ProgramNode Compile(string sourceCode)
  {
    return TypeCheck(sourceCode).program;
  }

  /// <summary>
  /// Execute a source string.
  /// </summary>
  /// <param name="sourceCode"></param>
  /// <param name="colorize"></param>
  /// <returns></returns>
  public static string Execute(string sourceCode, bool colorize = true)
  {
    if (TryExecute(sourceCode, out var result, colorize))
    {
      return result;
    }
    return "";
  }

  /// <summary>
  /// Execute a source string.
  /// </summary>
  /// <param name="sourceCode"></param>
  /// <param name="result"></param>
  /// <param name="colorize"></param>
  /// <param name="stdin"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static bool TryExecute(
    string sourceCode,
    out string result,
    bool colorize = true,
    TextReader? stdin = null,
    CommandLineOptions? options = null
  )
  {
    options = options ?? CommandLineOptions.Default;

    if (options.lexons)
    {
      throw new NotImplementedException();
    }

    if (options.ast)
    {
      throw new NotImplementedException();
    }

    var program = Parse(sourceCode);

    if (options.transform)
    {
      result = program.PrintPretty();
      return true;
    }

    var typeContext = new TypeContext();
    program.GetFactoryType(typeContext);

    if (options.types)
    {
      result = program.PrintPretty(x =>
        [x.FactoryType.ToShortString(), x.GetNodeHoverString() ?? ""]
      );
      return true;
    }

    if (typeContext.Errors.Count() != 0)
    {
      var stringBuilder = new StringBuilder();
      foreach (var (position, length, message) in typeContext.Errors)
      {
        var errorMessage = $"({position}, {length}): {message}";
        if (colorize)
        {
          stringBuilder.AppendLine(errorMessage.Colorize(CColor.Red));
        }
        else
        {
          stringBuilder.AppendLine(errorMessage);
        }
      }
      result = stringBuilder.ToString();
      return false;
    }

    using var textWriter = new StringWriter();
    using var context = new ExecutionContext(null!, program, stdin ?? TextReader.Null, textWriter);
    program!.Evaluate(context);

    result = textWriter.ToString().TrimEnd();
    return true;
  }

  public static void Execute(
    ModuleResolver moduleResolver,
    string entryPoint,
    TextReader input,
    TextWriter output
  )
  {
    var compiler = new Compiler.Compiler(moduleResolver);
    var program = compiler.CompileRoot(entryPoint);

    TypeContext typeContext = new TypeContext(program, program.EntryModule);
    program.EntryModule.GetFactoryType(typeContext);

    ExecutionContext executionContext = new ExecutionContext(
      program,
      program.EntryModule,
      input,
      output
    );
    program.EntryModule.Evaluate(executionContext);
  }
}
