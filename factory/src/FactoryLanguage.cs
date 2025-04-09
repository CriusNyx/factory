using System.Text;
using SharpParse.Functional;
using SharpParse.Lexing;
using SharpParse.Parsing;
using SharpParse.Util;

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
    var lexons = FactoryLexer.LexFactory(sourceCode, true);
    return lexons
      .Map(x => (lexon: x, semanticType: x.GetSemanticType()))
      .Filter(x => x.semanticType != FactorySemanticType.whitespace)
      .Map(x => new FactorySemanticToken(
        x.lexon.index,
        x.lexon.length,
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
    var lexons = Lex(sourceCode);
    var owner = lexons.FirstOrDefault(x => x.HasIndex(index) && x.lexonType == FactoryLexon.symbol);
    if (owner == null)
    {
      return [];
    }
    var output = AutocompleteCache
      .Search(owner.sourceCode)
      .Concat(
        lexons
          .Filter(x => x.lexonType == FactoryLexon.symbol)
          .Map(x => x.sourceCode)
          .Where(x => x.StartsWith(owner.sourceCode))
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
      // Helper function to generate an error
      FactoryLanguageError LexonError(int start, int end)
      {
        var len = end - start;
        return new FactoryLanguageError(
          start,
          len,
          FactoryErrorType.error,
          $"Unrecognized symbol {sourceCode.Substring(start, len)}"
        );
      }

      var lexons = FactoryLexer.LexFactory(sourceCode, true);

      // Crawl lexons and check for errors
      for (int i = -1; i < lexons.Length; i++)
      {
        // No idea what's happening in here.
        // I think it's checking to match up the heads and tails of lexons to look for code that failed to lex.
        var a = lexons.SafeGet(i);
        var b = lexons.SafeGet(i + 1);
        if (a == null && b != null)
        {
          if (b.index != 0)
          {
            errors.Add(LexonError(0, b.index));
          }
        }
        else if (b == null && a != null)
        {
          if (a.end != sourceCode.Length)
          {
            errors.Add(LexonError(a.end, sourceCode.Length));
          }
        }
        else if (a != null && b != null)
        {
          if (a.end != b.index)
          {
            errors.Add(LexonError(a.end, b.index));
          }
        }
      }

      // Parse factory language
      var result = FactoryParser.TryParse(lexons);

      // On a failed result, transfer errors to output.
      if (result is FailedParseResult failed)
      {
        var lexon = failed.offendingLexon;
        var position = lexon?.index ?? sourceCode.Length;
        var length = lexon?.length ?? 0;
        string message = failed.ErrorMessage();

        errors.Add(new FactoryLanguageError(position, length, FactoryErrorType.error, message));
      }
      // If the program succeeded, transform and type check it.
      else if (result is SuccessParseResult succ)
      {
        var typeContext = new TypeContext();
        var program = Transformer.Transform(succ.astNode) as ProgramNode;
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
  /// Lex the factory language.
  /// </summary>
  /// <param name="sourceCode"></param>
  /// <param name="resumeAfterError"></param>
  /// <returns></returns>
  public static Lexon[] Lex(string sourceCode, bool resumeAfterError = false)
  {
    return FactoryLexer.LexFactory(sourceCode, resumeAfterError).Filter(x => x.isSemantic);
  }

  /// <summary>
  /// Parse the factory language.
  /// </summary>
  /// <param name="lexons"></param>
  /// <returns></returns>
  public static ASTNode Parse(Lexon[] lexons)
  {
    return FactoryParser.Parse(lexons)!;
  }

  /// <summary>
  /// Parse the factory language.
  /// </summary>
  /// <param name="sourceCode"></param>
  /// <returns></returns>
  public static ASTNode Parse(string sourceCode)
  {
    return Parse(Lex(sourceCode));
  }

  /// <summary>
  /// Transform the AST into a program node.
  /// </summary>
  /// <param name="astNode"></param>
  /// <returns></returns>
  public static ProgramNode Transform(ASTNode astNode)
  {
    var result = Transformer.Transform(astNode);
    if (result is ProgramNode program)
    {
      return program;
    }
    throw new InvalidCastException();
  }

  /// <summary>
  /// Transform the AST into a program node.
  /// </summary>
  /// <param name="sourceCode"></param>
  /// <returns></returns>
  public static ProgramNode Transform(string sourceCode)
  {
    return Transform(Parse(sourceCode));
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
    return TypeCheck(Transform(sourceCode));
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
    var lexons = Lex(sourceCode);

    if (options.lexons)
    {
      result = string.Join(
        "\n",
        lexons.Map(x => $"{x.lexonType.ToString().PadRight(20)} {x.sourceCode}")
      );
      return true;
    }

    var ast = Parse(lexons);

    if (options.ast)
    {
      result = ast.PrintProgram();
      return true;
    }

    var program = Transform(ast);

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
