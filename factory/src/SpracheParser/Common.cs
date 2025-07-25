using SharpParse.Functional;

namespace SpracheParser;

public interface ASTNode : ProgramEquivalent { }

public interface Statement : ASTNode { }

public interface ProgramEquivalent
{
  bool Equivalent(ProgramEquivalent other);
  public static bool Equivalent(ProgramEquivalent a, ProgramEquivalent b)
  {
    return a.Equivalent(b);
  }
}

public static class ProgramEquivalentExtensions
{
  public static bool SafeEquivalent(this ProgramEquivalent? a, ProgramEquivalent? b)
  {
    return a.EquivalentOrNull(b, ProgramEquivalent.Equivalent);
  }

  public static bool Equivalent(
    this IEnumerable<ProgramEquivalent> self,
    IEnumerable<ProgramEquivalent> other
  )
  {
    return self.OuterZip(other).All(x => x.Item1.SafeEquivalent(x.Item2));
  }
}

public interface RightHandExpression : ASTNode, ProgramEquivalent { }
