namespace Factory;

public class TypeContext
{
  Dictionary<string, FactoryType> symbolResolutions = new Dictionary<string, FactoryType>();
  Stack<FactoryType> typeStack = new Stack<FactoryType>();
  List<(int position, int length, string message)> errors =
    new List<(int position, int length, string error)>();

  public IEnumerable<(int position, int length, string message)> Errors => errors;

  public void SetType(string symbol, FactoryType type)
  {
    symbolResolutions[symbol] = type;
  }

  public FactoryType GetType(string symbol)
  {
    if (symbolResolutions.TryGetValue(symbol, out var result))
    {
      return result;
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
}
