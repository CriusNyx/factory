using System.Text;
using GenParse.Functional;

namespace GenParse.Util
{
  public static class Formatting
  {
    public static string TreeIndent(int indent)
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
      return indentString;
    }

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
      string indentString = TreeIndent(indent);
      builder.AppendLine(indentString + elementName(root));
      foreach (var child in getChildren(root))
      {
        PrintTree(child, elementName, getChildren, builder, indent + 1);
      }
    }

    public static string PrintGrid(
      string[][] lines,
      string columnSeparator = "  | ",
      bool[]? alignRight = null,
      string eol = ""
    )
    {
      var height = lines.Length;
      var width = lines.Reduce(0, (arr, value) => Math.Max(arr.Length, value));
      int[] columnWidths = new int[width];

      alignRight = alignRight ?? columnWidths.Map(x => false);

      // Compute width of each column.
      for (int columnIndex = 0; columnIndex < width; columnIndex++)
      {
        int columnWidth = 0;
        for (int lineIndex = 0; lineIndex < height; lineIndex++)
        {
          var element = lines[lineIndex][columnIndex];
          columnWidth = Math.Max(columnWidth, element.Length);
        }
        columnWidths[columnIndex] = columnWidth;
      }

      // Construct output
      StringBuilder builder = new StringBuilder();

      for (int lineIndex = 0; lineIndex < height; lineIndex++)
      {
        string GridString(string element, int index)
        {
          int w = columnWidths[index];
          bool right = alignRight[index];
          return right ? element.PadLeft(w) : element.PadRight(w);
        }
        var line = lines[lineIndex].Map(GridString);
        Func<string, StringBuilder> builderFunc =
          lineIndex == height - 1 ? builder.Append : builder.AppendLine;

        builderFunc($"{string.Join(columnSeparator, line)}{eol}");
      }

      // Remove trailing space.
      builder.Remove(builder.Length - 1, 1);

      return builder.ToString();
    }

    internal static string ToLiteral(string valueTextForCompiler)
    {
      return Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(valueTextForCompiler, false);
    }
  }
}
