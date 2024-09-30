using GenParse.Functional;
using GenParse.Parsing;

public class Transformer<LexonType, LanguageNode> where LanguageNode: class
{

  Dictionary<string, Func<Transformer<LexonType, LanguageNode>, ASTNode<LexonType>, LanguageNode>> rules
    = new Dictionary<string, Func<Transformer<LexonType, LanguageNode>, ASTNode<LexonType>, LanguageNode>>();

  public Transformer((string, Func<Transformer<LexonType, LanguageNode>, ASTNode<LexonType>, LanguageNode>)[] rules){
    this.rules = rules.ToDictionary()!;
  }


  public LanguageNode? Transform(ASTNode<LexonType> astNode)
  {
    if (astNode == null)
    {
      return null;
    }
    return rules.Safe(astNode.name)?.Invoke(this, astNode);
  }

  public LanguageNode[] TransformChildren(ASTNode<LexonType> astNode)
  {
    return astNode?.children.Map(Transform).FilterNull() ?? [];
  }

  public LanguageNode[] TransformAll(ASTNode<LexonType> astNode, string search) => TransformAll(astNode, [search]);

  public LanguageNode[] TransformAll(ASTNode<LexonType> astNode, string[] search)
  {
    return astNode?.MatchAll(search).Map(Transform).FilterNull() ?? [];
  }

  public string GetSymbol(ASTNode<LexonType> astNode)
  {
    var node = astNode?.MatchAll("symbol").FirstNotNull();
    return node?.lexons[0].sourceCode ?? "";
  }
}
