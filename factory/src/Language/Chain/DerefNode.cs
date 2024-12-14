using System.Reflection;
using GenParse.Util;

namespace Factory;

[ASTClass("Deref")]
public class DerefNode : LanguageNode, ChainNode
{
  [ASTField("symbol")]
  public SymbolNode derefSymbol;

  public FactVal Evaluate(FactVal target, ExecutionContext context)
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
            return new FuncVal((args) => FuncVal.InvokeCSharpMethod(target, method, args)!);
          }
        }
      }
    }

    throw new NotImplementedException();
  }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [derefSymbol];
  }

  public string GetIdentifier()
  {
    return derefSymbol.symbolName;
  }
}
