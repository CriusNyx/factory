
using GenParse.Functional;
using GenParse.Lexing;

namespace GenParse.Parsing;

public class Parser<LexonType>
{
  readonly ParseContext<LexonType> context;

  public Parser(ParseContext<LexonType> context)
  {
    this.context = context;
  }

  public ASTNode<LexonType>? Parse(string rootSymbol, Lexon<LexonType>[] lexons)
  {
    return ParseStatic(context, rootSymbol, lexons);
  }

  public override string ToString()
  {
    return string.Join("\n", context.productionSets.Values.Select(x => x.ToString()));
  }

  static ASTNode<LexonType>? ParseStatic(ParseContext<LexonType> context, string rootSymbol, Lexon<LexonType>[] lexons)
  {
    var productionSet = context.productionSets.Safe(rootSymbol);
    if (productionSet != null)
    {
      var result = ParseProductionSet(context, productionSet, lexons, 0);
      return result?.astNode;
    }
    return null;
  }

  static ParseResult<LexonType>? ParseProductionSet(ParseContext<LexonType> context, ProductionSet<LexonType>? productionSet, Lexon<LexonType>[] lexons, int index)
  {
    return productionSet?.rules.FirstNotNull((rule) => ParseProductionRule(context, rule, lexons, index));
  }

  static ParseResult<LexonType>? ParseProductionRule(ParseContext<LexonType> context, ProductionRule<LexonType> productionRule, Lexon<LexonType>[] lexons, int index)
  {
    int offset = 0;
    List<ASTNode<LexonType>> nodes = new List<ASTNode<LexonType>>();
    foreach (var symbol in productionRule.symbols)
    {
      var lexon = lexons.SafeGet(index + offset);
      if (TryParseProductionSymbol(context, symbol, lexons, index + offset, out var result))
      {
        nodes.Add(result!.astNode);
        offset += result.lexonsConsumed;
      }
      else
      {
        return null;
      }
    }

    return new ParseResult<LexonType>(new ASTNode<LexonType>(productionRule.name, productionRule, nodes.ToArray(), []), offset);
  }

  static bool TryParseProductionSymbol(
    ParseContext<LexonType> context,
    ProductionSymbol<LexonType> symbol,
    Lexon<LexonType>[] lexons,
    int index,
    out ParseResult<LexonType>? result)
  {
    if (symbol.modifier != null)
    {
      switch (symbol.modifier)
      {
        case '*':
          {
            //result = symbol.ParseStar(context, lexons, index);
            result = ParseStar(context, symbol, lexons, index);
            return true;
          }
        case '?':
          {
            //result = symbol.ParseQuestion(context, lexons, index);
            result = ParseQuestion(context, symbol, lexons, index);
            return true;
          }
        default:
          throw new NotImplementedException();
      }
    }
    else
    {
      result = ParseSingle(context, symbol, lexons, index);
      return result != null;
    }
  }

  static ParseResult<LexonType> ParseStar(
    ParseContext<LexonType> context,
    ProductionSymbol<LexonType> symbol,
    Lexon<LexonType>[] lexons,
    int index)
  {
    int offset = 0;
    ParseResult<LexonType>? node;
    var output = new List<ASTNode<LexonType>>();
    do
    {
      node = ParseSingle(context, symbol, lexons, index + offset);
      if (node != null)
      {
        offset += node.lexonsConsumed;
        output.Add(node.astNode);
      }
    } while (node != null);
    return new ParseResult<LexonType>(new ASTNode<LexonType>($"{symbol.name}{symbol.modifier}", null, output.ToArray(), []), offset);
  }

  static ParseResult<LexonType>? ParseQuestion(
    ParseContext<LexonType> context,
    ProductionSymbol<LexonType> symbol,
    Lexon<LexonType>[] lexons,
    int index)
  {
    var result = ParseSingle(context, symbol, lexons, index);
    ASTNode<LexonType>[] children = [];
    if (result != null)
    {
      children = [result.astNode];
    }
    return new ParseResult<LexonType>(new ASTNode<LexonType>(symbol.NameWithMod, null, children, []), result?.lexonsConsumed ?? 0);
  }

  static ParseResult<LexonType>? ParseSingle(
    ParseContext<LexonType> context,
    ProductionSymbol<LexonType> symbol,
    Lexon<LexonType>[] lexons,
    int index)
  {
    if (symbol.isLexon)
    {
      if (lexons.TryGet(index, out var lexon))
      {

        if(Equals(lexon.lexonType, symbol.lexonType))
        {
          return new ParseResult<LexonType>(new ASTNode<LexonType>(symbol.name, null, [], lexons[index..(index + 1)]), 1);
        }
      }
    }
    else
    {
      var productionSet = context.productionSets.Safe(symbol.name);
      if (productionSet != null)
      {
        return ParseProductionSet(context, productionSet, lexons, index);
      }
    }
    return null;
  }
}