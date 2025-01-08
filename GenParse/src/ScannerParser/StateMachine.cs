using GenParse.Functional;
using GenParse.Lexing;
using GenParse.Parsing;

public class Scanner<T>
{
  public void Scan(Lexon<T>[] lexons)
  {
    ScannerSymbol<T>[] symbols = lexons.Map(x => new LexonSymbol<T>(x));
  }
}

public class StateMachine<T>(ProductionRule<T> productionRule)
{
  public readonly ProductionRule<T> productionRule = productionRule;

  public bool Match(ScannerSymbol<T>[] symbols, int index, out int lexonConsumed)
  {
    lexonConsumed = 0;
    foreach (var prodSym in productionRule.symbols)
    {
      if (MatchSym(prodSym, symbols, index, out var cons))
      {
        lexonConsumed += cons;
      }
      else
      {
        return false;
      }
    }
    return true;
  }

  private bool MatchSym(
    ProductionSymbol<T> prodSym,
    ScannerSymbol<T>[] symbols,
    int index,
    out int lexonsConsumed
  )
  {
    if (prodSym.modifier == null)
    {
      lexonsConsumed = 1;
      var sym = symbols[index];
      return sym.symbolType == prodSym.name;
    }
    lexonsConsumed = 0;
    switch (prodSym.modifier)
    {
      case '*':
        for (int i = index; i < symbols.Length; i++)
        {
          var sym = symbols[i];
          if (sym.symbolType == prodSym.name)
          {
            lexonsConsumed++;
          }
          else
            break;
        }
        return true;
      case '?':
        {
          var sym = symbols[index];
          if (sym.symbolType == prodSym.name)
          {
            lexonsConsumed++;
          }
        }
        return true;
      default:
        throw new NotImplementedException();
    }
  }
}

public abstract class ScannerSymbol<T>
{
  public abstract string symbolType { get; }
}

public class ASTSymbol<T>(ASTNode<T> astNode) : ScannerSymbol<T>
{
  public ASTNode<T> astNode = astNode;
  public override string symbolType => astNode.name;
}

public class LexonSymbol<T>(Lexon<T> lexon) : ScannerSymbol<T>
{
  public override string symbolType => lexon.lexonType.ToString()!;
  public Lexon<T> lexon = lexon;
}
