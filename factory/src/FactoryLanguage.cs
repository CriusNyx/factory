using Factory.Parsing;

public static class FactoryLanguage
{
  public static string Run(string sourceCode)
  {
    var lexons = FactoryLexer.LexFactory(sourceCode);
    var ast = FactoryParser.Parse(lexons)!;
    var program = Transformer.Transform(ast) as ProgramNode;
    using var textWriter = new StringWriter();
    using var context = new ExecutionContext(Console.In, textWriter);

    program!.Evaluate(context);

    return textWriter.ToString().TrimEnd();
  }
}
