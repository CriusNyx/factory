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
    var argTypes = parameters.Map(x => x.CalculateType(context));

    if (current is CSharpType cSharpType)
    {
      var type = cSharpType.type;
      if (type.IsAssignableTo(typeof(IFunc)))
      {
        var methodInfo = type.GetMethod("Invoke").NotNull();
        var resultInfo = methodInfo.GetCustomAttribute<InvocationTypeAttribute>().NotNull();
        var typeEvaluator = methodInfo
          .GetCustomAttribute<ArgumentTypeEvaluatorAttribute>()
          .NotNull();
        CheckArgsForErrors(argTypes, typeEvaluator.CheckTypes, context);
        if (resultInfo?.outType != null)
        {
          return FactoryType.FromCSharpType(resultInfo.outType);
        }
      }
    }
    if (current is MethodType methodType)
    {
      var type = methodType.outType;
      CheckArgsForErrors(argTypes, methodType.typeEvaluator.CheckTypes, context);

      return FactoryType.FromCSharpType(type);
    }
    var pos = ast.CalculatePosition();
    context.AddError(pos.start, pos.length, $"Cannot invoke on type {current}");
    return FactoryType.VoidType;
  }

  private void CheckArgsForErrors(
    FactoryType[] argTypes,
    Func<FactoryType[], bool[]> evaluator,
    TypeContext typeContext
  )
  {
    var typesAreValid = evaluator(argTypes);

    foreach (var (parameter, argType, valid) in parameters.Zip(argTypes, typesAreValid))
    {
      if (!valid)
      {
        var argErrorPos = parameter.astNode.CalculatePosition();
        typeContext.AddError(
          argErrorPos.start,
          argErrorPos.length,
          $"Argument of type {argType} is not valid."
        );
      }
    }
  }
}
