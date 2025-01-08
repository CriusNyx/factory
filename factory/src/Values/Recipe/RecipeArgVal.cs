using Factory;

public abstract class RecipeArgVal : FactVal { }

public abstract class RecipeArgValBase(string identifier) : RecipeArgVal
{
  public readonly string identifier = identifier;

  public override bool Equals(object? obj)
  {
    return obj is RecipeArgValBase @base && identifier == @base.identifier;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(identifier);
  }
}

public class InVal(string identifier) : RecipeArgValBase(identifier)
{
  public override string ToString()
  {
    return identifier;
  }
}

public class OutVal(string identifier) : RecipeArgValBase(identifier)
{
  public override string ToString()
  {
    return identifier;
  }
}

public class AltVal(string identifier) : RecipeArgValBase(identifier)
{
  public override string ToString()
  {
    return identifier;
  }
}

public class TallyVal(string identifier, bool inline) : RecipeArgValBase(identifier)
{
  public readonly bool inline = inline;

  public override bool Equals(object? obj)
  {
    return obj is TallyVal val && base.Equals(obj) && inline == val.inline;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(base.GetHashCode(), inline);
  }
}

public class LimitVal(string identifier, NumVal value) : RecipeArgValBase(identifier)
{
  public readonly NumVal value = value;

  public override bool Equals(object? obj)
  {
    return obj is LimitVal val
      && base.Equals(obj)
      && EqualityComparer<NumVal>.Default.Equals(value, val.value);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(base.GetHashCode(), value);
  }
}
