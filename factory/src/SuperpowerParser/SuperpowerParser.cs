using FactorySpracheParser;
using SharpParse.Functional;
using SpracheParser;
using Superpower;
using Superpower.Parsers;

namespace SuperpowerParser;

public static class FactoryParser
{
  public static readonly TokenListParser<SpracheTokenType, Statement> StatementParser = Parse.Ref(
    () =>
      Parse
        .OneOf(AssignParser.NotNull(), LineParser.NotNull().Select(x => x as Statement))
        .ThenIgnore(Token.EqualTo(SpracheTokenType.semicolon))
  );

  // Symbol
  public static readonly TokenListParser<SpracheTokenType, Symbol> SymbolParser = Parse.Ref(
    () => Token.EqualTo(SpracheTokenType.symbol).Select(x => new Symbol(x.ToSpracheToken()))
  );

  // Literal Expressions
  public static readonly TokenListParser<
    SpracheTokenType,
    RightHandExpression
  > NumberLiteralParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SpracheTokenType.numberLiteral)
        .Select(x => new NumberLiteral(x.ToSpracheToken()) as RightHandExpression)
  );

  public static readonly TokenListParser<
    SpracheTokenType,
    RightHandExpression
  > StringLiteralParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SpracheTokenType.stringLiteral)
        .Select(x => new StringLiteral(x.ToSpracheToken()) as RightHandExpression)
  );

  public static readonly TokenListParser<
    SpracheTokenType,
    RightHandExpression
  > LiteralExpressionParser = Parse.Ref(
    () => Parse.OneOf(NumberLiteralParser, StringLiteralParser)
  );

  // Line
  public static readonly TokenListParser<SpracheTokenType, ProductionLine> LineParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SpracheTokenType.lineKeyword)
        .IgnoreThen(SymbolParser)
        .Then(
          (name) =>
            LineStatementParser
              .NotNull()
              .Many()
              .Select(statements => new ProductionLine(name, statements))
        )
  );

  // Line Expressions
  public static readonly TokenListParser<SpracheTokenType, LineExpression> InParser = Parse.Ref(
    () => SimpleLineExpressionParser(SpracheTokenType.inKeyword, LineExpressionType.@in)
  );

  public static readonly TokenListParser<SpracheTokenType, LineExpression> OutParser = Parse.Ref(
    () => SimpleLineExpressionParser(SpracheTokenType.outKeyword, LineExpressionType.@out)
  );

  public static readonly TokenListParser<SpracheTokenType, LineExpression> AltParser = Parse.Ref(
    () => SimpleLineExpressionParser(SpracheTokenType.altKeyword, LineExpressionType.alt)
  );

  // Tally Expression
  public static readonly TokenListParser<SpracheTokenType, LineExpression> TallyParser = Parse.Ref(
    () =>
      from inline in Token
        .EqualTo(SpracheTokenType.tallyKeyword)
        .IgnoreThen(
          Token.EqualTo(SpracheTokenType.inlineKeyword).Optional().Select(x => x.HasValue)
        )
      from tallySyms in SimpleLineExpressionValueParser(LineExpressionType.tally).Many()
      select new LineExpression(LineExpressionType.tally, tallySyms, inline)
  );

  // Limit Expression
  public static readonly TokenListParser<SpracheTokenType, LineValueExpression> LimitValueParser =
    Parse.Ref(
      () =>
        from scalar in RightHandExpressionParser
        from name in SymbolParser
        select new LineValueExpression(LineExpressionType.limit, name, scalar)
    );

  public static readonly TokenListParser<SpracheTokenType, LineExpression> LimitParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SpracheTokenType.limitKeyword)
        .IgnoreThen(LimitValueParser.Many())
        .Select(x => new LineExpression(LineExpressionType.limit, x))
  );

  // Line Spread Statement
  public static readonly TokenListParser<SpracheTokenType, LineStatement> LineSpreadParser =
    Parse.Ref(
      () => SpreadParser.NotNull().Select(x => new LineSpreadStatement(x) as LineStatement)
    );

  // Line Expression
  public static readonly TokenListParser<SpracheTokenType, LineExpression> LineExpressionParser =
    Parse.Ref(() => Parse.OneOf(InParser, OutParser, AltParser, TallyParser, LimitParser));

  public static readonly TokenListParser<SpracheTokenType, LineStatement> LineStatementParser =
    Parse.Ref(
      () => Parse.OneOf(LineExpressionParser.Select(x => x as LineStatement), LineSpreadParser)
    );

  // Assign
  public static readonly TokenListParser<SpracheTokenType, Statement> AssignParser = Parse.Ref(
    () =>
      from lhs in LeftHandExpressionParser.NotNull()
      from rhs in Token
        .EqualTo(SpracheTokenType.equalSign)
        .IgnoreThen(RightHandExpressionParser.NotNull())
      select new AssignExpression(lhs, rhs) as Statement
  );

  // Unary Operators
  public static readonly TokenListParser<
    SpracheTokenType,
    RightHandExpression
  > NegateOperatorParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SpracheTokenType.minus)
        .IgnoreThen(
          UnitParser
            .NotNull()
            .Select(x => new UnaryExpression(UnaryOperation.realNegate, x) as RightHandExpression)
        )
  );

  public static readonly TokenListParser<
    SpracheTokenType,
    RightHandExpression
  > UnaryOperatorParser = Parse.Ref(() => Parse.OneOf(NegateOperatorParser, UnitParser.NotNull()));

  // Terms
  public static readonly TokenListParser<SpracheTokenType, BinaryOperation> FactorOperatorParser =
    Parse.Ref(
      () =>
        Parse.OneOf(
          Token.EqualTo(SpracheTokenType.asterisk).Value(BinaryOperation.mul),
          Token.EqualTo(SpracheTokenType.forwardSlash).Value(BinaryOperation.div),
          Token.EqualTo(SpracheTokenType.percent).Value(BinaryOperation.mod)
        )
    );

  public static readonly TokenListParser<SpracheTokenType, RightHandExpression> TermParser =
    Parse.Ref(
      () =>
        UnaryOperatorParser.Chain(
          FactorOperatorParser,
          UnaryOperatorParser,
          (op, x, y) => new BinaryExpression(op, x, y)
        )
    );

  // Math Expression
  public static readonly TokenListParser<SpracheTokenType, BinaryOperation> TermOperatorParser =
    Parse.Ref(
      () =>
        Parse.OneOf(
          Token.EqualTo(SpracheTokenType.plus).Value(BinaryOperation.add),
          Token.EqualTo(SpracheTokenType.minus).Value(BinaryOperation.sub)
        )
    );

  public static readonly TokenListParser<
    SpracheTokenType,
    RightHandExpression
  > MathExpressionParser = Parse.Ref(
    () =>
      TermParser.Chain(TermOperatorParser, TermParser, (op, x, y) => new BinaryExpression(op, x, y))
  );

  public static readonly TokenListParser<
    SpracheTokenType,
    RightHandExpression
  > ParentheticalParser = Parse.Ref(
    () =>
      MathExpressionParser
        .Between(
          Token.EqualTo(SpracheTokenType.openParen),
          Token.EqualTo(SpracheTokenType.closedParen)
        )
        .Select(x => new ParentheticalUnit(x) as RightHandExpression)
  );

  // Units
  public static readonly TokenListParser<SpracheTokenType, RightHandExpression> UnitParser =
    Parse.Ref(
      () =>
        Parse.OneOf(
          LiteralExpressionParser,
          LeftHandExpressionParser.NotNull().Select(x => x as RightHandExpression),
          ParentheticalParser
        )
    );

  // Deref
  public static readonly TokenListParser<SpracheTokenType, LHSChain> DerefParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SpracheTokenType.dot)
        .IgnoreThen(SymbolParser)
        .Select(x => new Deref(x) as LHSChain)
  );

  // Invocation
  public static readonly TokenListParser<SpracheTokenType, LHSChain> InvocationParser = Parse.Ref(
    () =>
      RightHandExpressionParser
        .NotNull()
        .SeparatedBy(Token.EqualTo(SpracheTokenType.comma))
        .Between(
          Token.EqualTo(SpracheTokenType.openParen),
          Token.EqualTo(SpracheTokenType.closedParen)
        )
        .Select(x => new Invocation(x.ToArray()) as LHSChain)
  );

  // Left Hand Expression
  public static readonly TokenListParser<SpracheTokenType, LHSChain> ChainParser = Parse.Ref(
    () => Parse.OneOf(DerefParser, InvocationParser)
  );

  public static readonly TokenListParser<SpracheTokenType, LHSContinue> ContinueParser = Parse.Ref(
    () =>
      from chain in ChainParser
      from next in ContinueParser.NotNull().Optional()
      select new LHSContinue(chain, next)
  );

  public static readonly TokenListParser<
    SpracheTokenType,
    LeftHandExpression
  > LeftHandExpressionParser = Parse.Ref(
    () =>
      from symbol in SymbolParser
      from chainContinue in ContinueParser.NotNull().Optional()
      select new LeftHandExpression(symbol, chainContinue)
  );

  // Right Hand Expression
  // TODO: Recipe Expression missing
  public static readonly TokenListParser<
    SpracheTokenType,
    RightHandExpression
  > RightHandExpressionParser = Parse.Ref(
    () =>
      Parse.OneOf(
        LeftHandExpressionParser.Select(x => x as RightHandExpression),
        MathExpressionParser
      )
  );

  // Spread
  public static readonly TokenListParser<SpracheTokenType, SpreadExpression> SpreadParser =
    Parse.Ref(
      () =>
        Token
          .EqualTo(SpracheTokenType.spread)
          .IgnoreThen(LeftHandExpressionParser)
          .Select(x => new SpreadExpression(x))
    );

  // Methods
  public static T ParseString<T>(string source, TokenListParser<SpracheTokenType, T> parser)
  {
    return parser.AtEnd().Parse(SuperpowerTokenizer.Tokenize(source));
  }

  public static TokenListParser<SpracheTokenType, LineExpression> SimpleLineExpressionParser(
    SpracheTokenType keywordType,
    LineExpressionType expressionType
  )
  {
    return Token
      .EqualTo(keywordType)
      .IgnoreThen(SimpleLineExpressionValueParser(expressionType).Many())
      .Select(x => new LineExpression(expressionType, x));
  }

  public static TokenListParser<
    SpracheTokenType,
    LineValueExpression
  > SimpleLineExpressionValueParser(LineExpressionType expressionType)
  {
    return SymbolParser.Select(x => new LineValueExpression(expressionType, x));
  }
}

public static class SuperpowerParserExtensions
{
  public static SpracheToken ToSpracheToken(this Superpower.Model.Token<SpracheTokenType> source)
  {
    return new SpracheToken(
      source.Kind,
      source.ToStringValue(),
      source.Position.Absolute,
      source.Span.Length
    );
  }

  public static TokenListParser<SpracheTokenType, IEnumerable<T>> SeparatedBy<T, U>(
    this TokenListParser<SpracheTokenType, T> parser,
    TokenListParser<SpracheTokenType, U> separator
  )
  {
    return parser
      .Select(x => new[] { x } as IEnumerable<T>)
      .Chain(separator, parser, (_, x, y) => x.Concat([y]))
      .Or(Parse.Return<SpracheTokenType, IEnumerable<T>>(new T[] { } as IEnumerable<T>));
  }

  public static TokenListParser<SpracheTokenType, T> ThenIgnore<T, U>(
    this TokenListParser<SpracheTokenType, T> parser,
    TokenListParser<SpracheTokenType, U> ignored
  )
  {
    return from output in parser from _ in ignored select output;
  }

  public static TokenListParser<SpracheTokenType, T?> Optional<T>(
    this TokenListParser<SpracheTokenType, T> parser
  )
    where T : class
  {
    return parser.Try().Or(Parse.Return<SpracheTokenType, T?>(null)!)!;
  }

  public static TokenListParser<SpracheTokenType, bool> Flag<T>(
    this TokenListParser<SpracheTokenType, T> parser
  )
  {
    return Parse.OneOf(parser.Select(x => true), Parse.Return<SpracheTokenType, bool>(false));
  }
}
