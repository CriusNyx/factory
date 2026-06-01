using Factory;

public abstract class LineArgVal : FactVal { }

public abstract class LineArgValBase(string identifier) : LineArgVal
{
  public readonly string identifier = identifier;

  public override bool Equals(object? obj)
  {
    return obj is LineArgValBase @base && identifier == @base.identifier;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(identifier);
  }
}

public class InVal(string identifier) : LineArgValBase(identifier)
{
  public override string ToString()
  {
    return identifier;
  }
}

public class OutVal(string identifier) : LineArgValBase(identifier)
{
  public override string ToString()
  {
    return identifier;
  }
}

public class AltVal(string identifier) : LineArgValBase(identifier)
{
  public override string ToString()
  {
    return identifier;
  }
}

public class TallyVal(string identifier, bool inline) : LineArgValBase(identifier)
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

public class LimitVal(string identifier, NumVal value) : LineArgValBase(identifier)
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
