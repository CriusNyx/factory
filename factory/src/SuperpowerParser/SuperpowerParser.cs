using FactorySpracheParser;
using SharpParse.Functional;
using SpracheParser;
using Superpower;
using Superpower.Parsers;

namespace SuperpowerParser;

public static class FactoryParser
{
  public static readonly TokenListParser<SuperpowerTokenType, Statement> StatementParser =
    Parse.Ref(
      () =>
        Parse
          .OneOf(AssignParser.NotNull(), LineParser.NotNull().Select(x => x as Statement))
          .ThenIgnore(Token.EqualTo(SuperpowerTokenType.semicolon))
    );

  // Symbol
  public static readonly TokenListParser<SuperpowerTokenType, Symbol> SymbolParser = Parse.Ref(
    () => Token.EqualTo(SuperpowerTokenType.symbol).Select(x => new Symbol(x.ToSpracheToken()))
  );

  // Literal Expressions
  public static readonly TokenListParser<
    SuperpowerTokenType,
    RightHandExpression
  > NumberLiteralParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SuperpowerTokenType.numberLiteral)
        .Select(x => new NumberLiteral(x.ToSpracheToken()) as RightHandExpression)
  );

  public static readonly TokenListParser<
    SuperpowerTokenType,
    RightHandExpression
  > StringLiteralParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SuperpowerTokenType.stringLiteral)
        .Select(x => new StringLiteral(x.ToSpracheToken()) as RightHandExpression)
  );

  public static readonly TokenListParser<
    SuperpowerTokenType,
    RightHandExpression
  > LiteralExpressionParser = Parse.Ref(
    () => Parse.OneOf(NumberLiteralParser, StringLiteralParser)
  );

  // Line
  public static readonly TokenListParser<SuperpowerTokenType, ProductionLine> LineParser =
    Parse.Ref(
      () =>
        Token
          .EqualTo(SuperpowerTokenType.lineKeyword)
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
  public static readonly TokenListParser<SuperpowerTokenType, LineExpression> InParser = Parse.Ref(
    () => SimpleLineExpressionParser(SuperpowerTokenType.inKeyword, LineExpressionType.@in)
  );

  public static readonly TokenListParser<SuperpowerTokenType, LineExpression> OutParser = Parse.Ref(
    () => SimpleLineExpressionParser(SuperpowerTokenType.outKeyword, LineExpressionType.@out)
  );

  public static readonly TokenListParser<SuperpowerTokenType, LineExpression> AltParser = Parse.Ref(
    () => SimpleLineExpressionParser(SuperpowerTokenType.altKeyword, LineExpressionType.alt)
  );

  // Tally Expression
  public static readonly TokenListParser<SuperpowerTokenType, LineExpression> TallyParser =
    Parse.Ref(
      () =>
        from inline in Token
          .EqualTo(SuperpowerTokenType.tallyKeyword)
          .IgnoreThen(
            Token.EqualTo(SuperpowerTokenType.inlineKeyword).Optional().Select(x => x.HasValue)
          )
        from tallySyms in SimpleLineExpressionValueParser(LineExpressionType.tally).Many()
        select new LineExpression(LineExpressionType.tally, tallySyms, inline)
    );

  // Limit Expression
  public static readonly TokenListParser<
    SuperpowerTokenType,
    LineValueExpression
  > LimitValueParser = Parse.Ref(
    () =>
      from scalar in RightHandExpressionParser
      from name in SymbolParser
      select new LineValueExpression(LineExpressionType.limit, name, scalar)
  );

  public static readonly TokenListParser<SuperpowerTokenType, LineExpression> LimitParser =
    Parse.Ref(
      () =>
        Token
          .EqualTo(SuperpowerTokenType.limitKeyword)
          .IgnoreThen(LimitValueParser.Many())
          .Select(x => new LineExpression(LineExpressionType.limit, x))
    );

  // Line Spread Statement
  public static readonly TokenListParser<SuperpowerTokenType, LineStatement> LineSpreadParser =
    Parse.Ref(
      () => SpreadParser.NotNull().Select(x => new LineSpreadStatement(x) as LineStatement)
    );

  // Line Expression
  public static readonly TokenListParser<SuperpowerTokenType, LineExpression> LineExpressionParser =
    Parse.Ref(() => Parse.OneOf(InParser, OutParser, AltParser, TallyParser, LimitParser));

  public static readonly TokenListParser<SuperpowerTokenType, LineStatement> LineStatementParser =
    Parse.Ref(
      () => Parse.OneOf(LineExpressionParser.Select(x => x as LineStatement), LineSpreadParser)
    );

  // Assign
  public static readonly TokenListParser<SuperpowerTokenType, Statement> AssignParser = Parse.Ref(
    () =>
      from lhs in LeftHandExpressionParser.NotNull()
      from rhs in Token
        .EqualTo(SuperpowerTokenType.equalSign)
        .IgnoreThen(RightHandExpressionParser.NotNull())
      select new AssignExpression(lhs, rhs) as Statement
  );

  // Unary Operators
  public static readonly TokenListParser<
    SuperpowerTokenType,
    RightHandExpression
  > NegateOperatorParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SuperpowerTokenType.minus)
        .IgnoreThen(
          UnitParser
            .NotNull()
            .Select(x => new UnaryExpression(UnaryOperation.realNegate, x) as RightHandExpression)
        )
  );

  public static readonly TokenListParser<
    SuperpowerTokenType,
    RightHandExpression
  > UnaryOperatorParser = Parse.Ref(() => Parse.OneOf(NegateOperatorParser, UnitParser.NotNull()));

  // Terms
  public static readonly TokenListParser<
    SuperpowerTokenType,
    BinaryOperation
  > FactorOperatorParser = Parse.Ref(
    () =>
      Parse.OneOf(
        Token.EqualTo(SuperpowerTokenType.asterisk).Value(BinaryOperation.mul),
        Token.EqualTo(SuperpowerTokenType.forwardSlash).Value(BinaryOperation.div),
        Token.EqualTo(SuperpowerTokenType.percent).Value(BinaryOperation.mod)
      )
  );

  public static readonly TokenListParser<SuperpowerTokenType, RightHandExpression> TermParser =
    Parse.Ref(
      () =>
        UnaryOperatorParser.Chain(
          FactorOperatorParser,
          UnaryOperatorParser,
          (op, x, y) => new BinaryExpression(op, x, y)
        )
    );

  // Math Expression
  public static readonly TokenListParser<SuperpowerTokenType, BinaryOperation> TermOperatorParser =
    Parse.Ref(
      () =>
        Parse.OneOf(
          Token.EqualTo(SuperpowerTokenType.plus).Value(BinaryOperation.add),
          Token.EqualTo(SuperpowerTokenType.minus).Value(BinaryOperation.sub)
        )
    );

  public static readonly TokenListParser<
    SuperpowerTokenType,
    RightHandExpression
  > MathExpressionParser = Parse.Ref(
    () =>
      TermParser.Chain(TermOperatorParser, TermParser, (op, x, y) => new BinaryExpression(op, x, y))
  );

  public static readonly TokenListParser<
    SuperpowerTokenType,
    RightHandExpression
  > ParentheticalParser = Parse.Ref(
    () =>
      MathExpressionParser
        .Between(
          Token.EqualTo(SuperpowerTokenType.openParen),
          Token.EqualTo(SuperpowerTokenType.closedParen)
        )
        .Select(x => new ParentheticalUnit(x) as RightHandExpression)
  );

  // Units
  public static readonly TokenListParser<SuperpowerTokenType, RightHandExpression> UnitParser =
    Parse.Ref(
      () =>
        Parse.OneOf(
          LiteralExpressionParser,
          LeftHandExpressionParser.NotNull().Select(x => x as RightHandExpression),
          ParentheticalParser
        )
    );

  // Deref
  public static readonly TokenListParser<SuperpowerTokenType, LHSChain> DerefParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SuperpowerTokenType.dot)
        .IgnoreThen(SymbolParser)
        .Select(x => new Deref(x) as LHSChain)
  );

  // Invocation
  public static readonly TokenListParser<SuperpowerTokenType, LHSChain> InvocationParser =
    Parse.Ref(
      () =>
        RightHandExpressionParser
          .NotNull()
          .SeparatedBy(Token.EqualTo(SuperpowerTokenType.comma))
          .Between(
            Token.EqualTo(SuperpowerTokenType.openParen),
            Token.EqualTo(SuperpowerTokenType.closedParen)
          )
          .Select(x => new Invocation(x.ToArray()) as LHSChain)
    );

  // Left Hand Expression
  public static readonly TokenListParser<SuperpowerTokenType, LHSChain> ChainParser = Parse.Ref(
    () => Parse.OneOf(DerefParser, InvocationParser)
  );

  public static readonly TokenListParser<SuperpowerTokenType, LHSContinue> ContinueParser =
    Parse.Ref(
      () =>
        from chain in ChainParser
        from next in ContinueParser.NotNull().Optional()
        select new LHSContinue(chain, next)
    );

  public static readonly TokenListParser<
    SuperpowerTokenType,
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
    SuperpowerTokenType,
    RightHandExpression
  > RightHandExpressionParser = Parse.Ref(
    () =>
      Parse.OneOf(
        LeftHandExpressionParser.Select(x => x as RightHandExpression),
        MathExpressionParser
      )
  );

  // Spread
  public static readonly TokenListParser<SuperpowerTokenType, SpreadExpression> SpreadParser =
    Parse.Ref(
      () =>
        Token
          .EqualTo(SuperpowerTokenType.spread)
          .IgnoreThen(LeftHandExpressionParser)
          .Select(x => new SpreadExpression(x))
    );

  // Methods
  public static T ParseString<T>(string source, TokenListParser<SuperpowerTokenType, T> parser)
  {
    return parser.AtEnd().Parse(SuperpowerTokenizer.Tokenize(source));
  }

  public static TokenListParser<SuperpowerTokenType, LineExpression> SimpleLineExpressionParser(
    SuperpowerTokenType keywordType,
    LineExpressionType expressionType
  )
  {
    return Token
      .EqualTo(keywordType)
      .IgnoreThen(SimpleLineExpressionValueParser(expressionType).Many())
      .Select(x => new LineExpression(expressionType, x));
  }

  public static TokenListParser<
    SuperpowerTokenType,
    LineValueExpression
  > SimpleLineExpressionValueParser(LineExpressionType expressionType)
  {
    return SymbolParser.Select(x => new LineValueExpression(expressionType, x));
  }
}

public static class SuperpowerParserExtensions
{
  public static SpracheToken ToSpracheToken(this Superpower.Model.Token<SuperpowerTokenType> source)
  {
    return new SpracheToken(
      source.Kind,
      source.ToStringValue(),
      source.Position.Absolute,
      source.Span.Length
    );
  }

  public static TokenListParser<SuperpowerTokenType, IEnumerable<T>> SeparatedBy<T, U>(
    this TokenListParser<SuperpowerTokenType, T> parser,
    TokenListParser<SuperpowerTokenType, U> separator
  )
  {
    return parser
      .Select(x => new[] { x } as IEnumerable<T>)
      .Chain(separator, parser, (_, x, y) => x.Concat([y]))
      .Or(Parse.Return<SuperpowerTokenType, IEnumerable<T>>(new T[] { } as IEnumerable<T>));
  }

  public static TokenListParser<SuperpowerTokenType, T> ThenIgnore<T, U>(
    this TokenListParser<SuperpowerTokenType, T> parser,
    TokenListParser<SuperpowerTokenType, U> ignored
  )
  {
    return from output in parser from _ in ignored select output;
  }

  public static TokenListParser<SuperpowerTokenType, T?> Optional<T>(
    this TokenListParser<SuperpowerTokenType, T> parser
  )
    where T : class
  {
    return parser.Try().Or(Parse.Return<SuperpowerTokenType, T?>(null)!)!;
  }

  public static TokenListParser<SuperpowerTokenType, bool> Flag<T>(
    this TokenListParser<SuperpowerTokenType, T> parser
  )
  {
    return Parse.OneOf(parser.Select(x => true), Parse.Return<SuperpowerTokenType, bool>(false));
  }
}
