using System.Reflection;
using GenParse.Util;

namespace Factory;

[ASTClass("Deref")]
public class DerefNode : ChainNode
{
  [ASTField("symbol")]
  public SymbolNode derefSymbol;

  public override FactVal Evaluate(FactVal target, ExecutionContext context)
  {
    return EvaluateDereference(target, derefSymbol.symbolName);
  }

  public static FactVal EvaluateDereference(FactVal target, string fieldName)
  {
    var targetType = target.GetType();

    foreach (var member in targetType.GetMembers())
    {
      if (
        member.GetCustomAttribute<ExposeMemberAttribute>() is ExposeMemberAttribute exposeFieldAttr
      )
      {
        if (exposeFieldAttr.name == fieldName)
        {
          if (member is FieldInfo field)
          {
            return field.GetValue(target)!.AsFactVal();
          }
          if (member is PropertyInfo property)
          {
            return property.GetValue(target)!.AsFactVal();
          }
          if (member is MethodInfo method)
          {
            return new FuncVal(target, method, MethodType.FromCSharpMethod(method));
          }
        }
      }
    }

    throw new NotImplementedException();
  }

  public override IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [derefSymbol];
  }

  public override string GetIdentifier()
  {
    return derefSymbol.symbolName;
  }

  public override FactoryType CalculateType(TypeContext context)
  {
    derefSymbol.GetFactoryType(context);
    refType = context.Peek();

    if (refType is ReferenceType referenceType)
    {
      derefSymbol.refInfo = new RefInfo(referenceType.symbol, derefSymbol.symbolName);
    }

    var resolvedType = refType.ResolveType(context);
    if (resolvedType is CSharpType cSharpType)
    {
      derefSymbol.refInfo = new RefInfo(cSharpType.GetCSharpType().Name, derefSymbol.symbolName);
      foreach (var member in cSharpType.type.GetMembers())
      {
        var exposeAttr = member.GetCustomAttribute<ExposeMemberAttribute>();
        if (exposeAttr?.name == derefSymbol.symbolName)
        {
          var output = FactoryType.FromCSharpMember(member);
          derefSymbol.OverrideType(output);
          return output;
        }
      }
    }
    var pos = astNode.CalculatePosition();
    context.AddError(pos.start, pos.length, $"{refType} has no member {derefSymbol.symbolName}");
    return FactoryType.VoidType;
  }
}
