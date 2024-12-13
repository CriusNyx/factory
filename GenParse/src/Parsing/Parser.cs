using System.Data;
using GenParse.Functional;
using GenParse.Lexing;

namespace GenParse.Parsing;

public class Parser<LexonType>
{
  public readonly IReadOnlyDictionary<string, ProductionSet<LexonType>> productionSets =
    new Dictionary<string, ProductionSet<LexonType>>();
  public readonly IReadOnlyDictionary<string, CustomParser<LexonType>> customParsers =
    new Dictionary<string, CustomParser<LexonType>>();

  private static Dictionary<string, LexonType[]> headCache = new Dictionary<string, LexonType[]>();

  public Parser(
    ProductionSet<LexonType>[] productionSets,
    CustomParser<LexonType>[] customParsers = null!
  )
  {
    customParsers = customParsers ?? [];

    this.productionSets = productionSets.ToDictionary(x => x.name);
    this.customParsers = customParsers.ToDictionary(x => x.name);
  }

  public ASTNode<LexonType>? Parse(
    string rootSymbol,
    Lexon<LexonType>[] lexons,
    bool forgiving = false
  )
  {
    var parseResult = ParseStatic(rootSymbol, lexons);
    if (parseResult is SuccessParseResult<LexonType> succ)
    {
      if (succ.lexonsConsumed != lexons.Length && !forgiving)
      {
        throw new ParseException<LexonType>(lexons[succ.lexonsConsumed]);
      }
      return succ.astNode;
    }
    else
    {
      if (lexons.Length > 0)
      {
        throw new ParseException<LexonType>(lexons[0]);
      }
      else
        throw new EmptyProgramException();
    }
  }

  public override string ToString()
  {
    return string.Join("\n", productionSets.Values.Select(x => x.ToString()));
  }

  ParseResult<LexonType>? ParseStatic(string rootSymbol, Lexon<LexonType>[] lexons)
  {
    var productionSet = productionSets.Safe(rootSymbol);
    if (productionSet != null)
    {
      var result = ParseProductionSet(productionSet, lexons, 0);
      return result;
    }
    return null;
  }

  ParseResult<LexonType>? ParseProductionSet(
    ProductionSet<LexonType>? productionSet,
    Lexon<LexonType>[] lexons,
    int index
  )
  {
    return productionSet?.rules.FirstNotNull((rule) => ParseProductionRule(rule, lexons, index));
  }

  ParseResult<LexonType> ParseProductionRule(
    ProductionRule<LexonType> productionRule,
    Lexon<LexonType>[] lexons,
    int index
  )
  {
    int offset = 0;
    List<ASTNode<LexonType>> nodes = new List<ASTNode<LexonType>>();
    foreach (var symbol in productionRule.symbols)
    {
      var lexon = lexons.SafeGet(index + offset);
      if (TryParseProductionSymbol(symbol, lexons, index + offset, out var result))
      {
        nodes.Add(result!.astNode);
        offset += result.lexonsConsumed;
      }
      else
      {
        return null;
      }
    }

    return new SuccessParseResult<LexonType>(
      new ASTNode<LexonType>(productionRule.name, productionRule, nodes.ToArray(), []),
      offset
    );
  }

  bool TryParseProductionSymbol(
    ProductionSymbol<LexonType> symbol,
    Lexon<LexonType>[] lexons,
    int index,
    out SuccessParseResult<LexonType> result
  )
  {
    if (symbol.modifier != null)
    {
      switch (symbol.modifier)
      {
        case '*':
        {
          //result = symbol.ParseStar(context, lexons, index);
          result = (ParseStar(symbol, lexons, index) as SuccessParseResult<LexonType>)!;
          return true;
        }
        case '?':
        {
          //result = symbol.ParseQuestion(context, lexons, index);
          result = (ParseQuestion(symbol, lexons, index) as SuccessParseResult<LexonType>)!;
          return true;
        }
        default:
          throw new NotImplementedException();
      }
    }
    else
    {
      var single = ParseSingle(symbol, lexons, index);
      if (single is SuccessParseResult<LexonType> succ)
      {
        result = succ;
        return true;
      }
      result = null!;
      return false;
    }
  }

  ParseResult<LexonType> ParseStar(
    ProductionSymbol<LexonType> symbol,
    Lexon<LexonType>[] lexons,
    int index
  )
  {
    int offset = 0;
    ParseResult<LexonType>? node;
    var output = new List<ASTNode<LexonType>>();
    do
    {
      node = ParseSingle(symbol, lexons, index + offset);
      if (node is SuccessParseResult<LexonType> succ)
      {
        offset += succ.lexonsConsumed;
        output.Add(succ.astNode);
      }
    } while (node != null);
    return new SuccessParseResult<LexonType>(
      new ASTNode<LexonType>($"{symbol.name}{symbol.modifier}", null, output.ToArray(), []),
      offset
    );
  }

  ParseResult<LexonType>? ParseQuestion(
    ProductionSymbol<LexonType> symbol,
    Lexon<LexonType>[] lexons,
    int index
  )
  {
    var result = ParseSingle(symbol, lexons, index) as SuccessParseResult<LexonType>;
    ASTNode<LexonType>[] children = [];
    if (result != null)
    {
      children = [result.astNode];
    }
    return new SuccessParseResult<LexonType>(
      new ASTNode<LexonType>(symbol.NameWithMod, null, children, []),
      result?.lexonsConsumed ?? 0
    );
  }

  ParseResult<LexonType>? ParseSingle(
    ProductionSymbol<LexonType> symbol,
    Lexon<LexonType>[] lexons,
    int index
  )
  {
    if (symbol.isLexon)
    {
      if (lexons.TryGet(index, out var lexon))
      {
        if (Equals(lexon.lexonType, symbol.lexonType))
        {
          return new SuccessParseResult<LexonType>(
            new ASTNode<LexonType>(symbol.name, null, [], lexons[index..(index + 1)]),
            1
          );
        }
      }
    }
    else
    {
      if (customParsers.TryGetValue(symbol.name, out var customParser))
      {
        var result = customParser.Parse(this, lexons, index);
        if (result != null)
        {
          return result;
        }
      }
      var productionSet = productionSets.Safe(symbol.name);
      if (productionSet != null)
      {
        return ParseProductionSet(productionSet, lexons, index);
      }
    }
    return null;
  }

  public LexonType[] ComputeHead(string grammarElementName)
  {
    if (headCache.TryGetValue(grammarElementName, out var result))
    {
      return result;
    }
    var productionSet = productionSets.Safe(grammarElementName);

    LexonType[] output;

    if (productionSet == null)
    {
      output = [];
    }
    else
    {
      output = ComputeHeadForSet(productionSet);
    }
    headCache[grammarElementName] = output;
    return output;
  }

  private LexonType[] ComputeHeadForSet(ProductionSet<LexonType> productionSet)
  {
    return productionSet.rules.FlatMap(ComputeHeadForRule).Distinct().ToArray();
  }

  private LexonType[] ComputeHeadForRule(ProductionRule<LexonType> productionRule)
  {
    if (productionRule.symbols.Length == 0)
    {
      return [];
    }
    List<LexonType> output = new List<LexonType>();
    for (int i = 0; i < productionRule.symbols.Length; i++)
    {
      var sym = productionRule.symbols[i];
      switch (sym.modifier)
      {
        case '*':
        case '?':
          output.AddRange(ComputeHeadForSymbol(sym));
          break;
        default:
          output.AddRange(ComputeHeadForSymbol(sym));
          return output.ToArray();
      }
    }
    return output.ToArray();
  }

  private LexonType[] ComputeHeadForSymbol(ProductionSymbol<LexonType> productionSymbol)
  {
    if (productionSymbol.isLexon)
    {
      return [productionSymbol.lexonType];
    }
    else
    {
      return ComputeHead(productionSymbol.name);
    }
  }
}
