using Factory;
using Factory.Util;

public static class LanguageVisitor
{
  public static void Traverse(
    this LanguageNode node,
    Action<LanguageNode>? visit = null,
    Action<LanguageNode>? visitAfter = null
  )
  {
    visit?.Invoke(node);
    foreach (var child in node.GetChildren().Select(x => x as LanguageNode).WhereDefined())
    {
      Traverse(child, visit, visitAfter);
    }
    visitAfter?.Invoke(node);
  }
}
