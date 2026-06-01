using Factory.Util;

namespace Factory;

public class ReferenceType(string symbol, LanguageNode languageNode) : FactoryType
{
  public readonly LanguageNode languageNode = languageNode;
  public readonly string symbol = symbol.NotNull();

  public bool CanAcceptValue(FactoryType other)
  {
    throw new NotImplementedException();
  }

  public string ToShortString()
  {
    return $"typeof {symbol}";
  }
}
