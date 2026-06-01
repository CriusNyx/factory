using Factory.Util;

namespace Factory;

public class ExecutionContext : IDisposable
{
  public readonly FactoryProgram program;
  public readonly ProgramNode programNode;
  private readonly Dictionary<string, FactVal> globalValues = new Dictionary<string, FactVal>();
  public IReadOnlyDictionary<string, FactVal> GlobalValues => globalValues;
  public readonly TextReader standardIn;
  public readonly TextWriter standardOut;

  public ExecutionContext(
    FactoryProgram program,
    ProgramNode programNode,
    TextReader standardIn,
    TextWriter standardOut
  )
  {
    this.program = program;
    this.programNode = programNode;
    this.standardIn = standardIn;
    this.standardOut = standardOut;
  }

  public ExecutionContext(FactoryProgram program, ProgramNode programNode)
    : this(program, programNode, Console.In, Console.Out) { }

  public FactVal? Resolve(SymbolVal identifier)
  {
    return globalValues.Safe(identifier.symbol)
      ?? Docs.recipesByProductIdentifier.Safe(identifier.symbol)?.First();
  }

  public void Assign(string identifier, FactVal val)
  {
    globalValues[identifier] = val;
  }

  public void Dispose()
  {
    standardIn.Dispose();
    standardOut.Dispose();
  }

  public static ExecutionContext From(ExecutionContext other, ProgramNode programNode)
  {
    return new ExecutionContext(other.program, programNode, other.standardIn, other.standardOut);
  }
}
