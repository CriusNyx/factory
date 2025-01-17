using GenParse.Functional;

public class RefInfo(string? objectIdentifier, string? fieldName = null)
{
  public readonly string? objectIdentifier = objectIdentifier;
  public readonly string? fieldName = fieldName;

  public string ToShortString()
  {
    return string.Join(".", Functional.FromDefined(objectIdentifier, fieldName));
  }
}
