using System.Reflection;

public class ObjectRefVal(FactVal owner, string identifier) : RefVal
{
  public readonly FactVal owner = owner;
  public readonly string identifier = identifier;

  public void Set(FactVal factVal)
  {
    foreach (var member in owner.GetType().GetMembers())
    {
      if (member.GetCustomAttribute<ExposeMemberAttribute>() is ExposeMemberAttribute memberAttr)
      {
        if (memberAttr.name == identifier)
        {
          if (member is FieldInfo field)
          {
            field.SetValue(owner, factVal.ConvertToType(field.FieldType));
          }
          if (member is PropertyInfo property)
          {
            if (property.CanWrite)
            {
              property.SetValue(owner, factVal.ConvertToType(property.PropertyType));
            }
          }
        }
      }
    }
  }
}
