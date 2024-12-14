using GenParse.Functional;
using GenParse.Util;

namespace Factory;

[ASTClass("AssignExp")]
public class AssignExpNode : ProgramExp, LanguageNode
{
  [ASTField("ExpChain")]
  public ExpChainNode left;

  [ASTField("ValueExp")]
  public ValueNode right;

  public (FactVal? value, ExecutionContext context) Evaluate(ExecutionContext context)
  {
    var reference = left.GetReference(context);
    var result = right.Evaluate(ref context);
    reference.Set(result);
    return result.With(context);
  }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return new Formatting.ITree<LanguageNode>[] { left, right };
  }
}
