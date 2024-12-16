using System.Formats.Tar;
using System.Reflection;
using GenParse.Functional;
using GenParse.Parsing;
using GenParse.Util;

namespace Factory;

[ASTClass("Invocation")]
public class InvocationNode : LanguageNode, ChainNode
{
  [AST]
  public ASTNode<FactoryLexon> ast;

  [ASTField("InvocationParamSet")]
  public ValueNode[] parameters;

  public IEnumerable<Formatting.ITree<LanguageNode>> GetChildren()
  {
    return parameters;
  }

  public FactVal Evaluate(FactVal target, ExecutionContext context)
  {
    var invocationParams = parameters.Map(x => x.Evaluate(ref context)).ToArrayVal();

    if (target is IFunc func)
    {
      return func.Invoke(invocationParams);
    }
    return null!;
  }

  public string GetIdentifier()
  {
    throw new Exception("Invocation nodes cannot be converted to identifiers.");
  }

  public FactoryType CalculateType(TypeContext context)
  {
    var current = context.Peek().Resolve(context);
    if (current is CSharpType cSharpType)
    {
      var type = cSharpType.type;
      if (type.IsAssignableTo(typeof(IFunc)))
      {
        var methodInfo = type.GetMethod("Invoke");
        var resultInfo = methodInfo?.GetCustomAttribute<InvocationTypeAttribute>();
        if (resultInfo?.outType != null)
        {
          return FactoryType.FromCSharpType(resultInfo.outType);
        }
      }
    }
    if (current is MethodType methodType)
    {
      var type = methodType.outType;
      return FactoryType.FromCSharpType(type);
    }
    var pos = ast.CalculatePosition();
    context.AddError(pos.start, pos.length, $"Cannot invoke on type {current}");
    return FactoryType.VoidType;
  }
}
