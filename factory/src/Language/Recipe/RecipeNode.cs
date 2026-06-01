using System.Security.Cryptography.X509Certificates;
using CommandLine;
using Factory;
using Factory.Util;

public class RecipeNode : StatementNode
{
  public bool Alt;
  public SymbolNode Name;
  public QuantityNode[] Input;
  public QuantityNode[] Output;

  public RecipeNode(
    SourceCodeInfo sourceInfo,
    bool alt,
    SymbolNode name,
    QuantityNode[] input,
    QuantityNode[] output
  )
    : base(sourceInfo)
  {
    Alt = alt;
    Name = name;
    Input = input;
    Output = output;
  }

  protected override void CalculateStaticType(TypeContext context)
  {
    context.SetGlobalType(Name.symbolName, new RecipeType(this));
    base.CalculateStaticType(context);
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    var output = new RecipeType(this);
    Name.OverrideType(output);

    Name.refInfo = new RefInfo(Name.symbolName);

    foreach (var input in Input)
    {
      input.GetFactoryType(context);
    }
    foreach (var _output in Output)
    {
      _output.GetFactoryType(context);
    }

    context.SetLocalType(Name.symbolName, output);

    return FactoryType.VoidType;
  }

  public override (FactVal value, Factory.ExecutionContext context) Evaluate(
    Factory.ExecutionContext context
  )
  {
    var output = new RecipeVal(
      Name.symbolName,
      Alt,
      Input.Select(x => x.Evaluate(ref context).Cast<QuantityVal>()).ToArray(),
      Output.Select(x => x.Evaluate(ref context).Cast<QuantityVal>()).ToArray()
    );
    context.Assign(Name.symbolName, output);
    return output.With(context);
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [Name, .. Input, .. Output];
  }

  public override bool Equivalent(object other)
  {
    return other is RecipeNode recipeNode
      && Alt == recipeNode.Alt
      && Name.Equivalent(recipeNode.Name)
      && Input.SetEquivalent(recipeNode.Input)
      && Output.SetEquivalent(recipeNode.Output);
  }
}
