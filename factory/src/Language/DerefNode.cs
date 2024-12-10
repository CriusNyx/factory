using System.Reflection;
using GenParse.Functional;
using GenParse.Util;

[ASTClass("Deref")]
public class DerefNode : LanguageNode, ChainNode
{
  [ASTField("symbol")]
  public SymbolNode derefSymbol;

  public FactVal Evaluate(FactVal target, ExecutionContext context)
  {
    var fieldName = derefSymbol.symbolName;

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
        }
      }
    }

    throw new NotImplementedException();
  }

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return [derefSymbol];
  }
}
