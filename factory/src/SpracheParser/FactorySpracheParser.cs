using SharpParse.Functional;
using Sprache;
using SpracheParser;

namespace FactorySpracheParser;

[AttributeUsage(AttributeTargets.Field)]
class NonSemantic : Attribute { }

public enum SuperpowerTokenType
{
  // Non Semantic
  comment,
  whitespace,

  // Language Symbols
  spread,
  dot,
  comma,
  openParen,
  closedParen,
  equalSign,
  semicolon,

  // Math Symbols

  plus,
  minus,
  asterisk,
  forwardSlash,
  percent,

  // Keywords
  lineKeyword,
  altKeyword,
  outKeyword,
  printKeyword,
  tallyKeyword,
  inlineKeyword,
  inKeyword,
  limitKeyword,

  // Literals
  stringLiteral,
  numberLiteral,

  // Symbol
  symbol,
}

public class SpracheToken(SuperpowerTokenType type, string source, int index, int length)
  : ProgramEquivalent,
    ASTNode
{
  public SuperpowerTokenType Type => type;
  public string Source => source;
  public int Index => index;
  public int Length => length;

  public static SpracheToken CreateTestToken(SuperpowerTokenType type, string source)
  {
    return new SpracheToken(type, source, 0, 0);
  }

  public bool Equivalent(ProgramEquivalent other)
  {
    return other is SpracheToken token && Type == token.Type && Source == token.Source;
  }
}

public static class FactoryParser
{
  static Parser<SpracheToken> Tokenize(
    this Parser<IEnumerable<char>> parser,
    SuperpowerTokenType type
  )
  {
    return parser
      .Text()
      .Span()
      .Select(value => new SpracheToken(type, value.Value, value.Start.Pos, value.Length));
  }

  static Parser<SpracheToken> Semantic(this Parser<SpracheToken> parser)
  {
    return from _0 in skip.Many() from result in parser from _1 in skip.Many() select result;
  }

  static Parser<T> Ignore<T, U>(this Parser<T> source, Parser<U> target)
  {
    return from s in source from t in target select s;
  }

  // Non Semantic Parsers
  public static readonly Parser<SpracheToken> commentParser = Parse
    .String("//")
    .Concat(Parse.AnyChar.Until(Parse.LineTerminator))
    .Tokenize(SuperpowerTokenType.comment);

  public static readonly Parser<SpracheToken> whitespaceParser = Parse
    .WhiteSpace.Many()
    .Tokenize(SuperpowerTokenType.whitespace);

  public static readonly Parser<SpracheToken> skip = whitespaceParser.Or(commentParser);

  // Language Symbols
  public static readonly Parser<SpracheToken> spreadParser = Parse
    .String("...")
    .Tokenize(SuperpowerTokenType.spread)
    .Semantic();

  public static readonly Parser<SpracheToken> dotParser = Parse
    .String(".")
    .Tokenize(SuperpowerTokenType.dot)
    .Semantic();

  public static readonly Parser<SpracheToken> commaParser = Parse
    .String(",")
    .Tokenize(SuperpowerTokenType.comma)
    .Semantic();

  public static readonly Parser<SpracheToken> openParenParser = Parse
    .String("(")
    .Tokenize(SuperpowerTokenType.openParen)
    .Semantic();

  public static readonly Parser<SpracheToken> closedParenParser = Parse
    .String(")")
    .Tokenize(SuperpowerTokenType.closedParen)
    .Semantic();

  public static readonly Parser<SpracheToken> equalSignParser = Parse
    .String("=")
    .Tokenize(SuperpowerTokenType.equalSign)
    .Semantic();

  public static readonly Parser<SpracheToken> semicolonParser = Parse
    .String(";")
    .Tokenize(SuperpowerTokenType.semicolon)
    .Semantic();

  // Math Symbols

  public static readonly Parser<SpracheToken> plusParser = Parse
    .String("+")
    .Tokenize(SuperpowerTokenType.plus)
    .Semantic();

  public static readonly Parser<SpracheToken> minusParser = Parse
    .String("-")
    .Tokenize(SuperpowerTokenType.minus)
    .Semantic();

  public static readonly Parser<SpracheToken> asteriskParser = Parse
    .String("*")
    .Tokenize(SuperpowerTokenType.asterisk)
    .Semantic();

  public static readonly Parser<SpracheToken> forwardSlashParser = Parse
    .String("/")
    .Tokenize(SuperpowerTokenType.forwardSlash)
    .Semantic();

  public static readonly Parser<SpracheToken> percentParser = Parse
    .String("%")
    .Tokenize(SuperpowerTokenType.percent)
    .Semantic();

  // Keywords
  public static readonly Parser<SpracheToken> lineKeywordParser = Parse
    .String("line")
    .Tokenize(SuperpowerTokenType.lineKeyword)
    .Semantic();

  public static readonly Parser<SpracheToken> altKeywordParser = Parse
    .String("alt")
    .Tokenize(SuperpowerTokenType.altKeyword)
    .Semantic();

  public static readonly Parser<SpracheToken> outKeywordParser = Parse
    .String("out")
    .Tokenize(SuperpowerTokenType.outKeyword)
    .Semantic();

  public static readonly Parser<SpracheToken> printKeywordParser = Parse
    .String("print")
    .Tokenize(SuperpowerTokenType.printKeyword)
    .Semantic();

  public static readonly Parser<SpracheToken> tallyKeywordParser = Parse
    .String("tally")
    .Tokenize(SuperpowerTokenType.tallyKeyword)
    .Semantic();

  public static readonly Parser<SpracheToken> inlineKeywordParser = Parse
    .String("inline")
    .Tokenize(SuperpowerTokenType.inlineKeyword)
    .Semantic();

  public static readonly Parser<SpracheToken> inKeywordParser = Parse
    .String("in")
    .Tokenize(SuperpowerTokenType.inKeyword)
    .Semantic();

  public static readonly Parser<SpracheToken> limitKeywordParser = Parse
    .String("limit")
    .Tokenize(SuperpowerTokenType.limitKeyword)
    .Semantic();

  // Literals
  public static readonly Parser<SpracheToken> stringLiteralParser = Parse
    .Regex("\".*?\"")
    .Tokenize(SuperpowerTokenType.stringLiteral)
    .Semantic();

  public static readonly Parser<SpracheToken> numberLiteralParser = Parse
    .Decimal.Tokenize(SuperpowerTokenType.numberLiteral)
    .Semantic();

  // Symbol
  public static readonly Parser<SpracheToken> symbolTokenParser = Parse
    .Regex("\\p{L}\\w*")
    .Tokenize(SuperpowerTokenType.symbol)
    .Semantic();

  static Parser<SpracheToken>[] tokenParsers =>
    [
      spreadParser,
      dotParser,
      commaParser,
      openParenParser,
      closedParenParser,
      equalSignParser,
      plusParser,
      minusParser,
      asteriskParser,
      forwardSlashParser,
      percentParser,
      equalSignParser,
      lineKeywordParser,
      altKeywordParser,
      outKeywordParser,
      printKeywordParser,
      tallyKeywordParser,
      inlineKeywordParser,
      inKeywordParser,
      limitKeywordParser,
      stringLiteralParser,
      numberLiteralParser,
      symbolTokenParser,
      commentParser,
      whitespaceParser,
    ];

  public static Parser<SpracheToken> anyTokenParser => tokenParsers.Aggregate((x, y) => x.Or(y));

  // Line Expressions
  public static Parser<LineValueExpression> LineValueSymbolExpressionParser(
    LineExpressionType type
  ) => Parse.Ref(() => symbolParser.Select(x => new LineValueExpression(type, x, null)));

  public static Parser<LineExpression> LineSymbolExpressionParser(
    Parser<SpracheToken> keywordParser,
    LineExpressionType type
  ) =>
    Parse.Ref(
      () =>
        keywordParser.Then(
          (_) =>
            LineValueSymbolExpressionParser(type)
              .Many()
              .Select((x) => new LineExpression(type, x.ToArray()))
        )
    );

  // Out Expression
  public static readonly Parser<LineExpression> outExpressionParser = LineSymbolExpressionParser(
    outKeywordParser,
    LineExpressionType.@out
  );

  // In Expression
  public static readonly Parser<LineExpression> inExpressionParser = LineSymbolExpressionParser(
    inKeywordParser,
    LineExpressionType.@in
  );

  // Alt Expression
  public static readonly Parser<LineExpression> altExpressionParser = LineSymbolExpressionParser(
    altKeywordParser,
    LineExpressionType.alt
  );

  // Tally Expression
  public static readonly Parser<LineExpression> tallyExpressionParser = Parse.Ref(() =>
  {
    return from _ in tallyKeywordParser
      from inline in inlineKeywordParser.Optional().Select(x => x.IsDefined)
      from values in LineValueSymbolExpressionParser(LineExpressionType.tally).Many()
      select new LineExpression(LineExpressionType.tally, values.ToArray(), inline);
  });

  // Limit Expression
  public static readonly Parser<LineExpression> limitExpressionParser = Parse.Ref(
    () =>
      limitKeywordParser
        .Then(x => limitValueExpressionParser.Many())
        .Select(x => new LineExpression(LineExpressionType.limit, x.ToArray()))
  );

  public static readonly Parser<LineValueExpression> limitValueExpressionParser = Parse.Ref(
    () =>
      from value in rightHandExpressionParser
      from symbol in symbolParser
      select new LineValueExpression(LineExpressionType.limit, symbol, value)
  );

  // Recipe Expression
  static readonly Parser<LineExpression>[] recipeExpressionParsers =
  [
    outExpressionParser,
    inExpressionParser,
    altExpressionParser,
    tallyExpressionParser,
    limitExpressionParser,
  ];

  // Recipe Spread Statement
  public static readonly Parser<LineStatement> recipeSpreadStatementParser = Parse.Ref(
    () => spreadExpressionParser.Select(x => new LineSpreadStatement(x))
  );

  // Recipe Expression.

  public static readonly Parser<LineExpression> recipeExpressionParser = Parse.Ref(
    () => recipeExpressionParsers.Aggregate((x, y) => x.Or(y))
  );

  public static readonly Parser<LineStatement> recipeStatementParser = recipeExpressionParser.Or(
    recipeSpreadStatementParser
  );

  // Statement Expression
  public static readonly Parser<Statement> statementParser = Parse.Ref(
    () => assignParser.NotNull().Ignore(semicolonParser)
  );

  // Assign Expression
  public static readonly Parser<Statement> assignParser = Parse.Ref(() =>
  {
    return from lhs in leftHandExpressionParser
      from _0 in equalSignParser
      from rhs in rightHandExpressionParser
      select new AssignExpression(lhs, rhs);
  });

  // Symbol Expression
  public static readonly Parser<Symbol> symbolParser = Parse.Ref(
    () => symbolTokenParser.Select(x => new Symbol(x))
  );

  // Value expressions
  public static readonly Parser<RightHandExpression> rightHandExpressionParser = Parse.Ref(
    () => mathExpressionParser
  );

  // Math Expressions
  public static readonly Parser<SpracheToken> termOperatorParser = Parse.Ref(
    () => plusParser.Or(minusParser)
  );

  public static readonly Parser<RightHandExpression> mathExpressionParser = Parse.Ref(
    () =>
      Parse.ChainOperator(
        termOperatorParser,
        termParser,
        (op, x, y) => new BinaryExpression(BinaryExpression.ParseOp(op.Source), x, y)
      )
  );

  // Factors
  public static readonly Parser<RightHandExpression> factorParser = Parse.Ref(
    () =>
      minusParser
        .Then((x) => unitParser)
        .Select(x => new UnaryExpression(UnaryOperation.realNegate, x))
        .Or(unitParser)
  );

  // Terms
  public static readonly Parser<SpracheToken> factorOperatorParser = Parse.Ref(
    () => asteriskParser.Or(forwardSlashParser).Or(percentParser)
  );

  public static readonly Parser<RightHandExpression> termParser = Parse.Ref(
    () =>
      Parse.ChainOperator(
        factorOperatorParser,
        factorParser,
        (op, x, y) => new BinaryExpression(BinaryExpression.ParseOp(op.Source), x, y)
      )
  );

  // Units
  public static readonly Parser<RightHandExpression> parentheticalParser = Parse.Ref(
    () =>
      mathExpressionParser
        .Contained(openParenParser, closedParenParser)
        .Select(x => new ParentheticalUnit(x))
  );

  public static readonly Parser<RightHandExpression> unitParser = Parse.Ref(
    () => literalExpParser.Or(leftHandExpressionParser).Or(parentheticalParser)
  );

  // Chain Expression
  public static readonly Parser<LHSChain> chainParser = Parse.Ref(
    () => derefParser.Or(invocationParser)
  );

  public static readonly Parser<LHSContinue> lhsContinueParser = Parse.Ref(
    () =>
      from chain in chainParser
      from next in lhsContinueParser.Optional()
      select new LHSContinue(chain, next.GetOrDefault())
  );

  public static readonly Parser<LeftHandExpression> leftHandExpressionParser = Parse.Ref(
    () =>
      from symbol in symbolParser
      from chainContinue in lhsContinueParser.Optional()
      select new LeftHandExpression(symbol, chainContinue.GetOrDefault())
  );

  // Deref
  public static readonly Parser<LHSChain> derefParser = Parse.Ref(
    () => dotParser.Then(_ => symbolParser).Select(x => new Deref(x))
  );

  // Invocation
  public static readonly Parser<LHSChain> invocationParser = Parse.Ref(
    () =>
      rightHandExpressionParser
        .DelimitedBy(commaParser)
        .Or(Parse.Return<IEnumerable<ASTNode>>([]))
        .Contained(openParenParser, closedParenParser)
        .Select(x => new Invocation(x.ToArray()))
  );

  // Literal Expressions
  public static readonly Parser<RightHandExpression> numberLiteralExpParser = Parse.Ref(
    () => numberLiteralParser.Select(x => new NumberLiteral(x))
  );

  public static readonly Parser<RightHandExpression> stringLiteralExpParser = Parse.Ref(
    () => stringLiteralParser.Select(x => new StringLiteral(x))
  );

  public static readonly Parser<RightHandExpression> literalExpParser = Parse.Ref(
    () => numberLiteralExpParser.Or(stringLiteralExpParser)
  );

  // Spread
  public static readonly Parser<SpreadExpression> spreadExpressionParser = Parse.Ref(
    () => spreadParser.Then((x) => leftHandExpressionParser).Select(x => new SpreadExpression(x))
  );
}
