using GenParse.Functional;

public class ExecutionContext
{
  public readonly Dictionary<string, FactVal> GlobalValues = new Dictionary<string, FactVal>();
  public readonly TextReader standardIn;
  public readonly TextWriter standardOut;

  public ExecutionContext(TextReader standardIn, TextWriter standardOut)
  {
    this.standardIn = standardIn;
    this.standardOut = standardOut;
  }

  public ExecutionContext() : this(Console.In, Console.Out) { }

  public FactVal? Resolve(SymbolVal identifier)
  {
    return GlobalValues.Safe(identifier.symbol)
      ?? Docs.recipesByProductIdentifier.Safe(identifier.symbol)?.First();
  }
}
