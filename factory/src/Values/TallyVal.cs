public class TallyVal : SymbolVal
{
  public bool inline;

  public TallyVal(string symbolVal, bool inline) : base(symbolVal)
  {
    this.inline = inline;
  }
}
