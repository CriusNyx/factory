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
            return;
          }
          if (member is PropertyInfo property)
          {
            if (property.CanWrite)
            {
              property.SetValue(owner, factVal.ConvertToType(property.PropertyType));
              return;
            }
            else
            {
              throw new Exception(
                $"Property {identifier} can not be set on object {owner.GetType()} because it is readonly."
              );
            }
          }
        }
      }
    }
    throw new Exception($"Property {identifier} can not be set on object {owner.GetType()}.");
  }
}
