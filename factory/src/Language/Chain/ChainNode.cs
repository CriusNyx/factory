namespace Factory;

public abstract class ChainNode : LanguageNode
{
  public abstract FactVal Evaluate(FactVal target, ExecutionContext context);
  public abstract string GetIdentifier();
}
