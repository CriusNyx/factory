using System.Text;
using Factory.Superpower;
using Factory.Util;
using Superpower;
using Superpower.Model;

namespace Factory;

/// <summary>
/// Class to process factory language source code and produce results.
/// </summary>
public static class FactoryLanguage
{
  /// <summary>
  /// Analyze the semantic content of source code for Factory LSP.
  /// </summary>
  /// <param name="sourceCode"></param>
  /// <returns></returns>
  public static FactorySemanticToken[] AnalyzeSemanticTokens(string sourceCode)
  {
    var lexons = SuperpowerTokenizer.Tokenize(sourceCode, false).ToArray();
    return lexons
      .Map(x => (lexon: x, semanticType: x.GetSemanticType()))
      .Filter(x => x.semanticType != FactorySemanticType.whitespace)
      .Map(x => new FactorySemanticToken(
        x.lexon.Position.Absolute,
        x.lexon.Span.Length,
        x.semanticType,
        (int)x.lexon.GetSemanticModifier()
      ));
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

  public static string? GetHoverString(string sourceCode, int hoverIndex)
  {
    try
    {
      var (program, _) = TypeCheck(sourceCode);
      if (program == null)
      {
        return "";
      }

      return program.GetHoverString(hoverIndex);
    }
    catch
    {
      return null;
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

  /// <summary>
  /// Analyze the factory language for errors.
  /// </summary>
  /// <param name="sourceCode"></param>
  /// <returns></returns>
  public static FactoryLanguageError[] AnalyzeErrors(string sourceCode)
  {
    // Initialize output.
    List<FactoryLanguageError> errors = new List<FactoryLanguageError>();

    try
    {
      Token<SuperpowerTokenType>[] lexons = SuperpowerTokenizer.Tokenize(sourceCode).ToArray();

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

      // Parse factory language
      var result = SuperpowerParser.TryParse(sourceCode);

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
      }
      else
      {
        var typeContext = new TypeContext();
        var program = result.Value;

        program?.GetFactoryType(typeContext);
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
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
    return errors.ToArray();
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
    using var context = new ExecutionContext(stdin ?? TextReader.Null, textWriter);
    program!.Evaluate(context);

    result = textWriter.ToString().TrimEnd();
    return true;
  }
}
