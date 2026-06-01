using System.Diagnostics;
using Factory.Util;

namespace Factory;

public class ProgramNode : LanguageNode
{
  private string filePath;
  public string FilePath
  {
    get => filePath;
    set
    {
      Debug.Assert(filePath == null);
      filePath = value;
    }
  }
  public StatementNode[] statements;
  private Dictionary<string, FactoryType>? exportTypes = null;
  private Dictionary<string, FactVal>? exportValues = null;

  public ProgramNode() { }

  public ProgramNode(SourceCodeInfo sourceInfo, StatementNode[] expressions)
    : base(sourceInfo)
  {
    this.statements = expressions;
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren() =>
    statements.ToTypedArray<Formatting.ITree<LanguageNode>>();

  public void Evaluate(ExecutionContext executionContext)
  {
    foreach (var expression in statements)
    {
      (_, executionContext) = expression.Evaluate(executionContext);
    }
  }

  public string ToTree()
  {
    return Formatting.PrintTree(this, x => x.ToString()!);
  }

  public IReadOnlyDictionary<string, FactoryType> GetExportedTypes(TypeContext original)
  {
    if (exportTypes == null)
    {
      // Initialize this before calculating type to avoid circular import issues.
      exportTypes = new Dictionary<string, FactoryType>();
      var typeContext = TypeContext.From(original, this);
      GetFactoryType(typeContext);
      exportTypes.AddRange(typeContext.GetExports());
    }
    return exportTypes;
  }

  public IReadOnlyDictionary<string, FactVal> GetExportedValues(ExecutionContext context)
  {
    if (exportValues == null)
    {
      exportValues = new Dictionary<string, FactVal>();
      var subContext = ExecutionContext.From(context, this);
      Evaluate(subContext);
      exportValues.AddRange(subContext.GlobalValues);
    }
    return exportValues;
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    CalculateStaticType(context);
    foreach (var statement in statements)
    {
      statement.GetFactoryType(context);
    }
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Void);
  }

  public string[] ResolveImports()
  {
    return statements
      .Select(x => x as ImportNode)
      .WhereDefined()
      .Select(x => x.Import.StringValue)
      .ToArray();
  }

  public override bool Equivalent(object other)
  {
    return other is ProgramNode program && statements.SetEquivalent(program.statements);
  }
}

public abstract class StatementNode : ValueNode
{
  public StatementNode() { }

  public StatementNode(SourceCodeInfo sourceInfo)
    : base(sourceInfo) { }
}
