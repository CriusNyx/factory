public interface ChainNode : LanguageNode
{
  public FactVal Evaluate(FactVal target, ExecutionContext context);
}
