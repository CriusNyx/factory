namespace Factory;

public class ResourceVal(string resourceName) : FactVal, HasResource
{
  public ResourceVal Resource => this;
  public string ResourceName => resourceName;
}
