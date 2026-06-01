using Factory;
using Factory.Util;

public class ImportNode : StatementNode
{
  public StringLiteralNode Import { get; private set; }

  public ImportNode() { }

  public ImportNode(SourceCodeInfo sourceInfo, StringLiteralNode import)
  {
    SetSourceInfo(sourceInfo);
    Import = import;
  }

  public override (FactVal value, Factory.ExecutionContext context) Evaluate(
    Factory.ExecutionContext context
  )
  {
    var exportedValues = context
      .program.ResolveModule(context.programNode.FilePath, Import.StringValue)
      .NotNull()
      .GetExportedValues(context);
    exportedValues.ForEach(x => context.Assign(x.Key, x.Value));
    return (null!, context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [Import];
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    var module = context
      .program.ResolveModule(context.programNode.FilePath, Import.StringValue)
      .NotNull();

    context.SetLocalTypes(module.GetExportedTypes(context));

    return FactoryType.VoidType;
  }

  public override bool Equivalent(object other)
  {
    return other is ImportNode import && Import.Equivalent(import.Import);
  }
}
