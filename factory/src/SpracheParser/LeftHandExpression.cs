using FactorySpracheParser;
using SharpParse.Functional;
using SpracheParser;

public interface LHSChain : ProgramEquivalent, ASTNode { }

public class LeftHandExpression(Symbol symbol, LHSContinue? next)
  : ProgramEquivalent,
    RightHandExpression
{
  public Symbol Symbol => symbol;
  public LHSContinue? Next => next;

  public bool Equivalent(ProgramEquivalent other)
  {
    if (other is LeftHandExpression expChain)
    {
      return Symbol.Equivalent(expChain.Symbol) && Next.SafeEquivalent(expChain.Next);
    }
    return false;
  }
}

public class Deref(Symbol symbol) : LHSChain
{
  public Symbol Symbol => symbol;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is Deref deref && Symbol.Equivalent(deref.Symbol);
  }
}

public class Invocation(ASTNode[] parameters) : LHSChain
{
  public ASTNode[] Parameters => parameters;

  public bool Equivalent(ProgramEquivalent other)
  {
    if (other is Invocation invocation)
    {
      return Parameters
        .OuterZip(invocation.Parameters)
        .All(
          (pair) =>
          {
            var (a, b) = pair;
            return a.SafeEquivalent(b);
          }
        );
    }
    return false;
  }
}

public class LHSContinue(LHSChain chain, LHSContinue? next) : ProgramEquivalent, ASTNode
{
  public LHSChain Chain => chain;

  public LHSContinue? Next => next;

  public bool Equivalent(ProgramEquivalent other)
  {
    if (other is LHSContinue chainContinue)
    {
      return Chain.Equivalent(chainContinue.Chain) && Next.SafeEquivalent(chainContinue.Next);
    }

    return false;
  }
}

public class Symbol(SpracheToken symbolToken) : ProgramEquivalent, RightHandExpression
{
  public SpracheToken SymbolToken => symbolToken;

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is Symbol symbol && SymbolToken.Equivalent(symbol.SymbolToken);
  }
}
