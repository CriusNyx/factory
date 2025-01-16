namespace Factory;

public abstract class ChainNode : LanguageNode
{
  public FactoryType refType { get; protected set; }
  public abstract FactVal Evaluate(FactVal target, ExecutionContext context);
  public abstract string GetIdentifier();
}
