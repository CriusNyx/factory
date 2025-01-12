namespace Factory;

[Flags]
public enum RecipeTypeType
{
  @in = 1,
  @out = 2,
  limit = 3,
  alt = 4,
  tally = 5,
  any = -1,
}

public class RecipeType(RecipeTypeType type) : FactoryType
{
  public readonly RecipeTypeType type = type;

  public bool CanAcceptValue(FactoryType other)
  {
    return other is RecipeType recType && (recType.type & type) != 0;
  }

  public string ToShortString()
  {
    return type.ToString();
  }
}
