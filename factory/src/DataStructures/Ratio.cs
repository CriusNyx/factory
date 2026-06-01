namespace Factory;

public class Ratio(RatioQuantity[] input, RatioQuantity[] output)
{
  public IEnumerable<RatioQuantity> Input => input;
  public IEnumerable<RatioQuantity> Output => output;

  public static Ratio BinOp(
    decimal value,
    Ratio ratio,
    Func<decimal, RatioQuantity, RatioQuantity> func
  )
  {
    return new Ratio(
      ratio.Input.Select(x => func(value, x)).ToArray(),
      ratio.Output.Select(x => func(value, x)).ToArray()
    );
  }

  public static Ratio operator *(decimal value, Ratio ratio)
  {
    return BinOp(value, ratio, (x, y) => x * y);
  }

  public static Ratio operator /(decimal value, Ratio ratio)
  {
    return BinOp(value, ratio, (x, y) => x / y);
  }

  public static Ratio operator /(Ratio ratio, decimal value)
  {
    return BinOp(value, ratio, (x, y) => y / x);
  }

  public RatioQuantity? PrimaryOutput()
  {
    return Output.First();
  }

  public Ratio Normalize()
  {
    return this / (PrimaryOutput()?.value ?? 1);
  }

  public Ratio Ordered()
  {
    return new Ratio(
      Input.OrderBy(x => x.identifier).ToArray(),
      Output.OrderBy(x => x.identifier).ToArray()
    );
  }

  public static bool Similar(Ratio self, Ratio other, decimal maxError = 0.0001m)
  {
    self = self.Ordered().Normalize();
    other = other.Ordered().Normalize();

    return self.Input.SequenceEqual(other.Input) && self.Output.SequenceEqual(other.Output);
  }

  public override string ToString()
  {
    return $"{PrintRatioArr(Input)} => {PrintRatioArr(Output)}";
  }

  private string PrintRatioArr(IEnumerable<RatioQuantity> collection)
  {
    if (collection == null || collection.Count() == 0)
    {
      return "_";
    }
    else
    {
      return string.Join(" + ", collection);
    }
  }
}

public struct RatioQuantity : IEquatable<RatioQuantity>
{
  public readonly decimal value;
  public readonly string identifier;

  public RatioQuantity(decimal value, string identifier)
  {
    this.value = value;
    this.identifier = identifier;
  }

  public static RatioQuantity BinOp(
    decimal value,
    RatioQuantity quantity,
    Func<decimal, decimal, decimal> op
  )
  {
    return new RatioQuantity(op(value, quantity.value), quantity.identifier);
  }

  public static RatioQuantity BinOp(
    RatioQuantity a,
    RatioQuantity b,
    Func<decimal, decimal, decimal> op
  )
  {
    if (a.identifier != b.identifier)
    {
      throw new InvalidOperationException(
        $"Cannot apply operator between quantity {a.identifier} and {b.identifier}"
      );
    }

    return new RatioQuantity(op(a.value, b.value), a.identifier);
  }

  public override bool Equals(object? obj)
  {
    return obj is RatioQuantity quantity && Equals(quantity);
  }

  public bool Equals(RatioQuantity other)
  {
    return value == other.value && identifier == other.identifier;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(value, identifier);
  }

  public static RatioQuantity operator +(decimal addend, RatioQuantity other) =>
    BinOp(addend, other, (x, y) => x + y);

  public static RatioQuantity operator +(RatioQuantity self, RatioQuantity other) =>
    BinOp(self, other, (x, y) => x + y);

  public static RatioQuantity operator -(RatioQuantity self)
  {
    return new RatioQuantity(-self.value, self.identifier);
  }

  public static RatioQuantity operator -(decimal addend, RatioQuantity other) =>
    BinOp(addend, other, (x, y) => x - y);

  public static RatioQuantity operator *(decimal scalar, RatioQuantity other) =>
    BinOp(scalar, other, (x, y) => x * y);

  public static RatioQuantity operator /(decimal scalar, RatioQuantity other) =>
    new RatioQuantity(scalar / other.value, other.identifier);

  public static RatioQuantity operator /(RatioQuantity self, decimal scalar) =>
    new RatioQuantity(self.value / scalar, self.identifier);

  public static bool operator ==(RatioQuantity left, RatioQuantity right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(RatioQuantity left, RatioQuantity right)
  {
    return !(left == right);
  }

  public override string ToString()
  {
    return $"{value:#.###} {identifier}";
  }
}
