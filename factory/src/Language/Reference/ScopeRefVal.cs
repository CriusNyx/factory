namespace Factory;

public class ScopeRefVal(ExecutionContext context, string identifier) : RefVal
{
  public readonly ExecutionContext context = context;
  public readonly string identifier = identifier;

  public void Set(FactVal factVal)
  {
    context.Assign(identifier, factVal);
  }
}
