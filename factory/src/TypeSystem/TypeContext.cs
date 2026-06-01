using Factory.Util;

namespace Factory;

public class TypeContext
{
  public FactoryProgram program { get; private set; }
  public ProgramNode programNode { get; private set; }
  Dictionary<string, FactoryType> staticSymbolResolutions = new Dictionary<string, FactoryType>();
  Dictionary<string, FactoryType> symbolResolutions = new Dictionary<string, FactoryType>();
  Stack<FactoryType> typeStack = new Stack<FactoryType>();
  List<(int position, int length, string message)> errors =
    new List<(int position, int length, string error)>();

  public TypeContext() { }

  public TypeContext(FactoryProgram program, ProgramNode programNode)
  {
    this.program = program;
    this.programNode = programNode;
  }

  public IEnumerable<(int position, int length, string message)> Errors => errors;

  public void SetGlobalType(string symbol, FactoryType type)
  {
    staticSymbolResolutions[symbol] = type;
  }

  public void SetLocalType(string symbol, FactoryType type)
  {
    symbolResolutions[symbol] = type;
  }

  public void SetLocalTypes(IReadOnlyDictionary<string, FactoryType> types)
  {
    symbolResolutions.AddRange(types);
  }

  public FactoryType GetType(string symbol)
  {
    if (symbolResolutions.TryGetValue(symbol, out var result))
    {
      return result;
    }

    if (staticSymbolResolutions.TryGetValue(symbol, out var staticResult))
    {
      return staticResult;
    }

    if (FactoryLanguage.ResolveGlobal(symbol) is object o)
    {
      return FactoryType.FromCSharpType(o.GetType());
    }

    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Void);
  }

  public void Push(FactoryType type)
  {
    typeStack.Push(type);
  }

  public FactoryType Peek() => typeStack.Peek();

  public FactoryType Pop() => typeStack.Pop();

  public void PopAll() => typeStack.Clear();

  public void AddError(int position, int length, string error)
  {
    errors.Add((position, length, error));
  }

  public static TypeContext From(TypeContext original, ProgramNode programNode)
  {
    return new TypeContext(original.program, programNode);
  }

  public void AcquireErrorsFrom(TypeContext other)
  {
    errors.AddRange(other.errors);
  }

  public Dictionary<string, FactoryType> GetExports()
  {
    var output = new Dictionary<string, FactoryType>();
    output.SafeAddRange(staticSymbolResolutions);
    output.SafeAddRange(symbolResolutions);
    return output;
  }
}
