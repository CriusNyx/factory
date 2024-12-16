using System.Data;
using System.Text;
using GenParse.Functional;
using GenParse.Lexing;
using GenParse.Parsing;
using GenParse.Util;
using Microsoft.VisualBasic;

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

  public static object? ResolveGlobal(string symbol)
  {
    return Docs.itemsByIdentifier.Safe(symbol);
  }

  public static Lexon<FactoryLexon>[] Lex(string sourceCode, bool resumeAfterError = false)
  {
    return FactoryLexer.LexFactory(sourceCode, resumeAfterError).Filter(x => x.isSemantic);
  }

  public static ASTNode<FactoryLexon> Parse(Lexon<FactoryLexon>[] lexons)
  {
    return FactoryParser.Parse(lexons)!;
  }

  public static ASTNode<FactoryLexon> Parse(string sourceCode)
  {
    return Parse(Lex(sourceCode));
  }

  public static ProgramNode Transform(ASTNode<FactoryLexon> astNode)
  {
    var result = Transformer.Transform(astNode);
    if (result is ProgramNode program)
    {
      return program;
    }
    throw new InvalidCastException();
  }

  public static ProgramNode Transform(string sourceCode)
  {
    return Transform(Parse(sourceCode));
  }

  public static (ProgramNode program, TypeContext typeContext) TypeCheck(ProgramNode program)
  {
    var typeContext = new TypeContext();
    program.CalculateType(typeContext);
    return (program, typeContext);
  }

  public static (ProgramNode program, TypeContext typeContext) TypeCheck(string sourceCode)
  {
    return TypeCheck(Transform(sourceCode));
  }

  public static ProgramNode Compile(string sourceCode)
  {
    return TypeCheck(sourceCode).program;
  }

  public static string Execute(string sourceCode, bool colorize = true)
  {
    if (TryExecute(sourceCode, out var result, colorize))
    {
      return result;
    }
    return "";
  }

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
      result = program!.ToTree();
      return true;
    }

    var typeContext = new TypeContext();
    program.CalculateType(typeContext);

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
