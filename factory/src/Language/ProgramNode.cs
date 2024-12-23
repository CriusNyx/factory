using GenParse.Functional;
using GenParse.Util;

namespace Factory;

[ASTClass("Program")]
public class ProgramNode : LanguageNode
{
  [ASTField("ProgramExp*")]
  public ProgramExp[] expressions;

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren() =>
    expressions.ToTypedArray<Formatting.ITree<LanguageNode>>();

  public void Evaluate(ExecutionContext executionContext)
  {
    foreach (var expression in expressions)
    {
      (_, executionContext) = expression.Evaluate(executionContext);
    }
  }

  public string ToTree()
  {
    return Formatting.PrintTree(this, x => x.ToString()!);
  }

  public FactoryType CalculateType(TypeContext context)
  {
    foreach (var expression in expressions)
    {
      expression.CalculateType(context);
    }
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Void);
  }
}

public interface ProgramExp : LanguageNode
{
  (FactVal? value, ExecutionContext context) Evaluate(ExecutionContext context);
}
