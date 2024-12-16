using GenParse.Functional;
using GenParse.Parsing;

namespace Factory;

public static class FactoryLanguage
{
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

  public static string[] GetSemanticTypes()
  {
    return Enum.GetValues<FactorySemanticType>().Map(x => x.ToString());
  }

  public static string[] GetSemanticModifiers()
  {
    // The number of valid flags is 1 less then the number of modifiers because None is not a flag.
    int count = Enum.GetValues<FactorySemanticModifier>().Length - 1;
    return Functional.Range(count).Map(x => ((FactorySemanticModifier)(1 << x)).ToString());
  }

  public static FactoryLanguageError[] AnalyzeErrors(string sourceCode)
  {
    List<FactoryLanguageError> errors = new List<FactoryLanguageError>();

    try
    {
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

      for (int i = -1; i < lexons.Length; i++)
      {
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

      var result = FactoryParser.TryParse(lexons);

      if (result is FailedParseResult<FactoryLexon> failed)
      {
        var lexon = failed.offendingLexon;
        var position = lexon?.index ?? sourceCode.Length;
        var length = lexon?.length ?? 0;
        string message = failed.ErrorMessage();

        errors.Add(new FactoryLanguageError(position, length, FactoryErrorType.error, message));
      }
      else if (result is SuccessParseResult<FactoryLexon> succ)
      {
        var typeContext = new TypeContext();
        var program = Transformer.Transform(succ.astNode) as ProgramNode;
        program?.CalculateType(typeContext);
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

  public static string Run(string sourceCode)
  {
    var lexons = FactoryLexer.LexFactory(sourceCode);
    var ast = FactoryParser.Parse(lexons)!;
    var program = Transformer.Transform(ast) as ProgramNode;
    using var textWriter = new StringWriter();
    using var context = new ExecutionContext(Console.In, textWriter);
    var typeContext = new TypeContext();
    program!.CalculateType(typeContext);

    if (typeContext.Errors.Count() != 0)
    {
      foreach (var error in typeContext.Errors)
      {
        textWriter.WriteLine(error);
      }
      return textWriter.ToString();
    }

    program.Evaluate(context);

    return textWriter.ToString().TrimEnd();
  }

  public static object? ResolveGlobal(string symbol)
  {
    return Docs.itemsByIdentifier.Safe(symbol);
  }
}
