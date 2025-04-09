using System.Reflection;
using SharpParse.Functional;

namespace Factory;

public enum FactoryPrimitiveTypeType
{
  Complex,
  Void,
  Number,
  String,
  Error,
}

public interface FactoryType
{
  bool CanAcceptValue(FactoryType other);

  string ToShortString();

  public static readonly FactoryType VoidType = new FactoryPrimitiveType(
    FactoryPrimitiveTypeType.Void
  );
  public static readonly FactoryType NumberType = new FactoryPrimitiveType(
    FactoryPrimitiveTypeType.Number
  );
  public static readonly FactoryType StringType = new FactoryPrimitiveType(
    FactoryPrimitiveTypeType.String
  );

  public static readonly FactoryType ErrorType = new FactoryPrimitiveType(
    FactoryPrimitiveTypeType.Error
  );

  public static FactoryType FromCSharpType(Type type)
  {
    if (type == typeof(string))
    {
      return FactoryType.StringType;
    }
    else if (type == typeof(void))
    {
      return FactoryType.VoidType;
    }
    else if (type == typeof(int) || type == typeof(float) || type == typeof(decimal))
    {
      return FactoryType.NumberType;
    }
    else if (type == typeof(StringVal))
    {
      return FactoryType.StringType;
    }
    else if (type == typeof(NumVal))
    {
      return FactoryType.NumberType;
    }
    else if (type.IsAssignableTo(typeof(object)))
    {
      return new CSharpType(type);
    }
    throw new NotImplementedException();
  }

  public static FactoryType FromCSharpMember(MemberInfo member)
  {
    if (member is FieldInfo field)
    {
      return FromCSharpType(field.FieldType);
    }
    else if (member is PropertyInfo property)
    {
      return FromCSharpType(property.PropertyType);
    }
    else if (member is MethodInfo method)
    {
      return MethodType.FromCSharpMethod(method);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  public static bool[] UnorderedTypeEvaluator(
    FactoryType[] argTypes,
    params FactoryType[] acceptableTypes
  )
  {
    return argTypes.Map(x => acceptableTypes.Any(y => y.CanAcceptValue(x)));
  }
}

public static class FactoryTypeExtensions
{
  public static FactoryType ResolveType(this FactoryType factoryType, TypeContext context)
  {
    if (factoryType is ReferenceType refType)
    {
      return context.GetType(refType.symbol);
    }
    return factoryType;
  }

  public static Type GetCSharpType(this FactoryType factoryType)
  {
    if (factoryType is CSharpType csType)
    {
      return csType.type;
    }
    throw new NotImplementedException();
  }

  public static bool IsEmpty(this FactoryType factoryType)
  {
    return factoryType is FactoryPrimitiveType primitiveType
      && (primitiveType.type == FactoryPrimitiveTypeType.Void);
  }
}
