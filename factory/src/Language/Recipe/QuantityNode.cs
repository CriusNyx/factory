using CommandLine;
using Factory;
using Factory.Util;

public class QuantityNode : ValueNode
{
  public NumberLiteralNode Quantity;
  public SymbolNode Item;

  public QuantityNode(SourceCodeInfo sourceInfo, NumberLiteralNode quantity, SymbolNode item)
  {
    SetSourceInfo(sourceInfo);
    Quantity = quantity;
    Item = item;
  }

  protected override FactoryType CalculateType(TypeContext context)
  {
    Quantity.GetFactoryType(context);
    Item.GetFactoryType(context);
    Item.refInfo = new RefInfo(Item.symbolName);
    return FactoryType.VoidType;
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [Quantity, Item];
  }

  public override bool Equivalent(object other)
  {
    return other is QuantityNode quantity
      && Quantity.Equivalent(quantity.Quantity)
      && Item.Equivalent(quantity.Item);
  }

  public override (FactVal value, Factory.ExecutionContext context) Evaluate(
    Factory.ExecutionContext context
  )
  {
    return new RatioQuantity(
      Quantity.Evaluate(ref context).Cast<NumVal>(),
      Item.Evaluate().Cast<SymbolVal>().symbol
    ).With(context);
  }
}
