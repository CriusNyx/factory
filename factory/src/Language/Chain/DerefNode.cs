using System.Formats.Tar;
using System.Reflection;
using System.Runtime.CompilerServices;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("Deref")]
public class DerefNode : LanguageNode, ChainNode
{
  [AST]
  public ASTNode<FactoryLexon> ast;

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

  public FactoryType CalculateType(TypeContext context)
  {
    var value = context.Peek().Resolve(context);
    if (value is CSharpType cSharpType)
    {
      foreach (var member in cSharpType.type.GetMembers())
      {
        var exposeAttr = member.GetCustomAttribute<ExposeMemberAttribute>();
        if (exposeAttr?.name == derefSymbol.symbolName)
        {
          return FactoryType.FromCSharpMember(member);
        }
      }
    }
    var pos = ast.CalculatePosition();
    context.AddError(pos.start, pos.length, $"{value} has no member {derefSymbol.symbolName}");
    return FactoryType.VoidType;
  }
}
