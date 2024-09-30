using System.Text;

namespace GenParse.Util
{
  public static class Formatting
  {
    public static string PrintTree<T>(
      T root,
      Func<T, string> elementName,
      Func<T, IEnumerable<T>> getChildren
    )
    {
      var builder = new StringBuilder();
      PrintTree(root, elementName, getChildren, builder, 0);
      return builder.ToString();
    }

    public static void PrintTree<T>(
      T root,
      Func<T, string> elementName,
      Func<T, IEnumerable<T>> getChildren,
      StringBuilder builder,
      int indent
    )
    {
      string indentString = "";
      for (int i = 0; i < indent - 1; i++)
      {
        indentString += "| ";
      }
      if (indent > 0)
      {
        indentString += "|-";
      }
      builder.AppendLine(indentString + elementName(root));
      foreach (var child in getChildren(root))
      {
        PrintTree(child, elementName, getChildren, builder, indent + 1);
      }
    }

    internal static string ToLiteral(string valueTextForCompiler)
    {
      return Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(valueTextForCompiler, false);
    }
  }
}
