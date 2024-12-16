using System.Reflection;

namespace Factory;

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
      return new MethodType(method.ReturnType);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  bool CanAcceptValue(FactoryType other);
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

public enum FactoryPrimitiveTypeType
{
  Complex,
  Void,
  Number,
  String,
}

public class FactoryPrimitiveType(FactoryPrimitiveTypeType type) : FactoryType
{
  public readonly FactoryPrimitiveTypeType type = type;

  public bool CanAcceptValue(FactoryType other)
  {
    switch (type)
    {
      case FactoryPrimitiveTypeType.Void:
        return false;
      case FactoryPrimitiveTypeType.String:
        return CanCoerceToString(other);
      case FactoryPrimitiveTypeType.Number:
        return other is FactoryPrimitiveType primType
          && primType.type == FactoryPrimitiveTypeType.Number;
      default:
        throw new NotImplementedException();
    }
  }

  private bool CanCoerceToString(FactoryType otherType)
  {
    if (otherType is FactoryPrimitiveType primType)
    {
      switch (primType.type)
      {
        case FactoryPrimitiveTypeType.Number:
        case FactoryPrimitiveTypeType.String:
          return true;
      }
    }
    return false;
  }

  public override string ToString()
  {
    return type.ToString();
  }
}

public class TypeContext
{
  Dictionary<string, FactoryType> symbolResolutions = new Dictionary<string, FactoryType>();
  Stack<FactoryType> typeStack = new Stack<FactoryType>();
  List<(int position, int length, string message)> errors =
    new List<(int position, int length, string error)>();

  public IEnumerable<(int position, int length, string message)> Errors => errors;

  public void SetType(string symbol, FactoryType type)
  {
    symbolResolutions[symbol] = type;
  }

  public FactoryType GetType(string symbol)
  {
    if (symbolResolutions.TryGetValue(symbol, out var result))
    {
      return result;
    }
    if (FactoryLanguage.ResolveGlobal(symbol) is object o)
    {
      return FactoryType.FromCSharpType(o.GetType());
    }
    return new FactoryPrimitiveType(FactoryPrimitiveTypeType.Void);
  }

  public void Push(FactoryType type)
  {
    typeStack.Push(type);
  }

  public FactoryType Peek() => typeStack.Peek();

  public FactoryType Pop() => typeStack.Pop();

  public void PopAll() => typeStack.Clear();

  public void AddError(int position, int length, string error)
  {
    errors.Add((position, length, error));
  }
}

public class CSharpType(Type type) : FactoryType
{
  public readonly Type type = type;

  public bool CanAcceptValue(FactoryType other)
  {
    throw new NotImplementedException();
  }

  public override string ToString()
  {
    return type.ToString();
  }
}

public class ReferenceType(string symbol) : FactoryType
{
  public readonly string symbol = symbol;

  public bool CanAcceptValue(FactoryType other)
  {
    throw new NotImplementedException();
  }
}

public enum RecipeTypeType
{
  @in,
  @out,
  limit,
  alt,
  tally,
}

public class RecipeType(RecipeTypeType type) : FactoryType
{
  public readonly RecipeTypeType type = type;

  public bool CanAcceptValue(FactoryType other)
  {
    return other is RecipeType recType && recType.type == type;
  }
}

public class InvocationTypeAttribute(Type outType) : Attribute
{
  public readonly Type outType = outType;
}

public class MethodType(Type outType) : FactoryType
{
  public readonly Type outType = outType;

  public bool CanAcceptValue(FactoryType other)
  {
    return other is MethodType methodType && methodType.outType == outType;
  }
}
