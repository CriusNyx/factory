using GenParse.Functional;

public class ExecutionContext
{
  public readonly Dictionary<string, object> GlobalValues = new Dictionary<string, object>();
  public readonly TextReader standardIn;
  public readonly TextWriter standardOut;

  public ExecutionContext(TextReader standardIn, TextWriter standardOut)
  {
    this.standardIn = standardIn;
    this.standardOut = standardOut;
  }

  public ExecutionContext() : this(Console.In, Console.Out) { }

  public object? Resolve(string identifier)
  {
    return GlobalValues.Safe(identifier)
      ?? Docs.recipesByProductIdentifier.Safe(identifier)?.First();
  }
}
