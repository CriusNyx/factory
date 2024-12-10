public class TallyVal : SymbolVal
{
  public bool inline;

  public TallyVal(string symbolVal, bool inline)
    : base(symbolVal)
  {
    this.inline = inline;
  }

  public override bool Equals(object? obj)
  {
    return obj is TallyVal val && base.Equals(obj) && inline == val.inline;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(base.GetHashCode(), inline);
  }
}
