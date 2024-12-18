using System.Reflection;
using GenParse.Functional;

namespace Factory;

public enum FactoryPrimitiveTypeType
{
  Complex,
  Void,
  Number,
  String,
}

public interface FactoryType
{
  public static readonly FactoryType VoidType = new FactoryPrimitiveType(
    FactoryPrimitiveTypeType.Void
  );
  public static readonly FactoryType NumberType = new FactoryPrimitiveType(
    FactoryPrimitiveTypeType.Number
  );
  public static readonly FactoryType StringType = new FactoryPrimitiveType(
    FactoryPrimitiveTypeType.String
  );

  public static readonly FactoryType InType = new RecipeType(RecipeTypeType.@in);
  public static readonly FactoryType OutType = new RecipeType(RecipeTypeType.@out);
  public static readonly FactoryType LimitType = new RecipeType(RecipeTypeType.limit);
  public static readonly FactoryType AltType = new RecipeType(RecipeTypeType.alt);
  public static readonly FactoryType TallyType = new RecipeType(RecipeTypeType.tally);
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
      var typeEvaluatorAttribute = method
        .GetCustomAttribute<ArgumentTypeEvaluatorAttribute>()
        .NotNull();
      return new MethodType(method.ReturnType, typeEvaluatorAttribute);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  bool CanAcceptValue(FactoryType other);

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
  public static FactoryType Resolve(this FactoryType factoryType, TypeContext context)
  {
    if (factoryType is ReferenceType refType)
    {
      return context.GetType(refType.symbol);
    }
    return factoryType;
  }
}
