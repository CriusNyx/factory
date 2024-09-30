using System.Diagnostics.CodeAnalysis;
using GenParse.Util;

namespace GenParse.Lexing;

public class Lexon<LexonType>
{
  [NotNull]
  public readonly LexonType lexonType;
  public readonly string sourceCode;
  public readonly bool isSemantic;

  public Lexon(LexonType lexonType, string sourceCode, bool isSemantic)
  {
    if (lexonType == null)
    {
      throw new ArgumentNullException(nameof(lexonType));
    }
    this.lexonType = lexonType;
    this.sourceCode = sourceCode;
    this.isSemantic = isSemantic;
  }

  public override string ToString()
  {
    return $"{lexonType} \"{Formatting.ToLiteral(sourceCode)}\"";
  }
}
