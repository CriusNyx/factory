using CommandLine;
using SharpParse.Functional;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Factory.Superpower;

public static class SuperpowerParser
{
  // Program

  public static TokenListParser<SuperpowerTokenType, ProgramNode> ProgramParser = Parse.Ref(
    () =>
      StatementParser
        .NotNull()
        .Many()
        .WithSourceInfo()
        .Select(x => new ProgramNode(x.sourceInfo, x.value))
  );

  // Statements
  public static TokenListParser<SuperpowerTokenType, ProgramExp> StatementParser = Parse.Ref(
    () => Parse.OneOf(LineParser.NotNull(), AssignExpParser.NotNull(), PrintParser.NotNull())
  );

  // Line Exp

  public static TokenListParser<SuperpowerTokenType, ProgramExp> LineParser = Parse.Ref(
    () =>
      (
        from _ in Token.EqualTo(SuperpowerTokenType.lineKeyword)
        from name in SymbolParser.NotNull()
        from expressions in LineExpParser.NotNull().Many()
        select (name, expressions)
      )
        .WithSourceInfo()
        .Select(x => new LineNode(x.sourceInfo, x.value.name, x.value.expressions) as ProgramExp)
  );

  public static TokenListParser<SuperpowerTokenType, LineExpNode> LineValueExpParser = Parse.Ref(
    () =>
      Parse.OneOf(
        InExpParser.NotNull(),
        OutExpParser.NotNull(),
        AltExpParser.NotNull(),
        TallyExpParser.NotNull(),
        LimitExpParser.NotNull()
      )
  );

  public static TokenListParser<SuperpowerTokenType, LineExpNode> LineExpParser = Parse.Ref(
    () => Parse.OneOf(LineValueExpParser, SpreadParser.NotNull().Select(x => x.Cast<LineExpNode>()))
  );

  public static TokenListParser<SuperpowerTokenType, LineExpNode> InExpParser = Parse
    .Ref(
      () => Token.EqualTo(SuperpowerTokenType.inKeyword).IgnoreThen(SymbolParser.NotNull().Many())
    )
    .WithSourceInfo()
    .Select(x => new InExpNode(x.sourceInfo, x.value) as LineExpNode);

  public static TokenListParser<SuperpowerTokenType, LineExpNode> OutExpParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SuperpowerTokenType.outKeyword)
        .IgnoreThen(SymbolParser.NotNull().Many())
        .WithSourceInfo()
        .Select(x => new OutExpNode(x.sourceInfo, x.value) as LineExpNode)
  );

  public static TokenListParser<SuperpowerTokenType, LineExpNode> AltExpParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SuperpowerTokenType.altKeyword)
        .IgnoreThen(SymbolParser.NotNull().Many())
        .WithSourceInfo()
        .Select(x => new AltExpNode(x.sourceInfo, x.value) as LineExpNode)
  );

  // Tally Exp
  public static TokenListParser<SuperpowerTokenType, LineExpNode> TallyExpParser = Parse.Ref(
    () =>
      (
        from _ in Token.EqualTo(SuperpowerTokenType.tallyKeyword)
        from inline in Token.EqualTo(SuperpowerTokenType.inlineKeyword).Flag()
        from expressions in SymbolParser.NotNull().Many()
        select (inline, expressions)
      )
        .WithSourceInfo()
        .Select(x =>
          new TallyExpNode(x.sourceInfo, x.value.inline, x.value.expressions) as LineExpNode
        )
  );

  // Limit
  public static TokenListParser<SuperpowerTokenType, LineExpNode> LimitExpParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SuperpowerTokenType.limitKeyword)
        .IgnoreThen(LimitValParser.NotNull().Many())
        .WithSourceInfo()
        .Select(x => new LimitExpNode(x.sourceInfo, x.value) as LineExpNode)
  );

  public static TokenListParser<SuperpowerTokenType, LimitValueExpNode> LimitValParser = Parse.Ref(
    () =>
      (from value in ValueExpParser from symbol in SymbolParser.NotNull() select (value, symbol))
        .WithSourceInfo()
        .Select(x => new LimitValueExpNode(x.sourceInfo, x.value.value, x.value.symbol))
        .Try()
  );

  // Print
  public static TokenListParser<SuperpowerTokenType, ProgramExp> PrintParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SuperpowerTokenType.printKeyword)
        .IgnoreThen(
          ValueExpParser.NotNull().SeparatedBy(Token.EqualTo(SuperpowerTokenType.comma).Optional())
        )
        .WithSourceInfo()
        .Select(x => PrintExpNode.Create(x.sourceInfo, x.value) as ProgramExp)
  );

  // AssignExp
  public static TokenListParser<SuperpowerTokenType, ProgramExp> AssignExpParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SuperpowerTokenType.letKeyword)
        .IgnoreThen(AssignParser.NotNull())
        .WithSourceInfo()
        .Select(x => AssignExpNode.Create(x.sourceInfo, x.value.lhs, x.value.rhs) as ProgramExp)
  );

  public static TokenListParser<
    SuperpowerTokenType,
    (ExpChainNode lhs, ValueNode rhs)
  > AssignParser = Parse.Ref(
    () =>
      (
        from lhs in ExpChainParser
        from _ in Token.EqualTo(SuperpowerTokenType.equalSign)
        from rhs in ValueExpParser.NotNull()
        select (lhs, rhs)
      )
  );

  // ValueExp
  public static TokenListParser<SuperpowerTokenType, ValueNode> ValueExpParser = Parse.Ref(
    () =>
      Parse
        .OneOf(
          LineValueExpParser.NotNull().Select(x => x.Cast<ValueNode>()),
          MathExpParser.NotNull()
        )
        .Try()
  );

  // Math
  public static TokenListParser<SuperpowerTokenType, ValueNode> MathExpParser = Parse.Ref(
    () =>
      (from term in TermParser from chain in TermChainParser.NotNull().Many() select (term, chain))
        .WithSourceInfo()
        .Select(x =>
          new MathExpNode(x.sourceInfo, x.value.term, x.value.chain).Simplify<ValueNode>()
        )
  );

  public static TokenListParser<SuperpowerTokenType, TermChainNode> TermChainParser = Parse.Ref(
    () =>
      (from op in TermOperatorParser from term in TermParser.NotNull() select (op, term))
        .WithSourceInfo()
        .Select(x => new TermChainNode(x.sourceInfo, x.value.op, x.value.term))
  );

  public static TokenListParser<SuperpowerTokenType, OperationNode> TermOperatorParser = Parse.Ref(
    () =>
      Parse
        .OneOf(Token.EqualTo(SuperpowerTokenType.plus), Token.EqualTo(SuperpowerTokenType.minus))
        .Select(x => new OperationNode(x.Info()))
  );

  // Term
  public static TokenListParser<SuperpowerTokenType, ValueNode> TermParser = Parse.Ref(
    () =>
      (
        from factor in FactorParser
        from chain in FactorChainParser.NotNull().Many()
        select (factor, chain)
      )
        .WithSourceInfo()
        .Select(x =>
          new TermNode(x.sourceInfo, x.value.factor, x.value.chain).Simplify<ValueNode>()
        )
  );

  public static TokenListParser<SuperpowerTokenType, FactorChainNode> FactorChainParser = Parse.Ref(
    () =>
      (from op in FactorOperatorParser from factor in FactorParser.NotNull() select (op, factor))
        .WithSourceInfo()
        .Select(x => new FactorChainNode(x.sourceInfo, x.value.op, x.value.factor))
  );

  // Factor
  public static readonly TokenListParser<SuperpowerTokenType, OperationNode> FactorOperatorParser =
    Parse.Ref(
      () =>
        Parse
          .OneOf(
            Token.EqualTo(SuperpowerTokenType.asterisk),
            Token.EqualTo(SuperpowerTokenType.forwardSlash),
            Token.EqualTo(SuperpowerTokenType.percent)
          )
          .Select(x => new OperationNode(x.Info()))
    );

  public static readonly TokenListParser<SuperpowerTokenType, ValueNode> FactorParser = Parse.Ref(
    () =>
      (
        from negate in Token.EqualTo(SuperpowerTokenType.minus).Flag()
        from unit in PrimitiveParser.NotNull()
        select (negate, unit)
      )
        .WithSourceInfo()
        .Select(x =>
          new FactorNode(x.sourceInfo, x.value.negate, x.value.unit).Simplify<ValueNode>()
        )
  );

  // Primitive
  public static readonly TokenListParser<SuperpowerTokenType, ValueNode> PrimitiveParser =
    Parse.Ref(
      () =>
        Parse.OneOf(
          ExpChainParser.NotNull().Select(x => x as ValueNode),
          LiteralParser.NotNull(),
          ParentheticalParser.NotNull()
        )
    );

  public static readonly TokenListParser<SuperpowerTokenType, ValueNode> ParentheticalParser =
    Parse.Ref(
      () =>
        MathExpParser
          .Between(
            Token.EqualTo(SuperpowerTokenType.openParen),
            Token.EqualTo(SuperpowerTokenType.closedParen)
          )
          .WithSourceInfo()
          .Select(x => new ParentheticalUnit(x.sourceInfo, x.value) as ValueNode)
    );

  // Exp Chain
  public static readonly TokenListParser<SuperpowerTokenType, ExpChainNode> ExpChainParser =
    Parse.Ref(
      () =>
        (
          from symbol in SymbolParser.NotNull()
          from cont in ContinueParser.NotNull()!.Optional()
          select (symbol, cont)
        )
          .WithSourceInfo()
          .Select(x => new ExpChainNode(x.sourceInfo, x.value.symbol, x.value.cont))
    );

  public static readonly TokenListParser<SuperpowerTokenType, ExpChainNode> ContinueParser =
    Parse.Ref(
      () =>
        (
          from chain in ChainParser.NotNull()
          from cont in ContinueParser.NotNull()!.Optional()
          select (chain, cont)
        )
          .WithSourceInfo()
          .Select(x => new ExpChainNode(x.sourceInfo, x.value.chain, x.value.cont))
    );

  // Chain
  public static readonly TokenListParser<SuperpowerTokenType, ChainNode> ChainParser = Parse.Ref(
    () => Parse.OneOf(DerefParser.NotNull(), InvocationParser.NotNull())
  );

  // Deref

  public static readonly TokenListParser<SuperpowerTokenType, ChainNode> DerefParser = Parse.Ref(
    () =>
      Token
        .EqualTo(SuperpowerTokenType.dot)
        .IgnoreThen(SymbolParser.NotNull())
        .WithSourceInfo()
        .Select(x => new DerefNode(x.sourceInfo, x.value) as ChainNode)
  );

  // Invocation
  public static readonly TokenListParser<SuperpowerTokenType, ChainNode> InvocationParser =
    Parse.Ref(
      () =>
        ValueExpParser
          .SeparatedBy(Token.EqualTo(SuperpowerTokenType.comma))
          .Between(
            Token.EqualTo(SuperpowerTokenType.openParen),
            Token.EqualTo(SuperpowerTokenType.closedParen)
          )
          .WithSourceInfo()
          .Select(x => new InvocationNode(x.sourceInfo, x.value) as ChainNode)
    );

  // Literals
  public static readonly TokenListParser<SuperpowerTokenType, ValueNode> StringLiteralParser =
    Parse.Ref(
      () =>
        Token
          .EqualTo(SuperpowerTokenType.stringLiteral)
          .Select(x => new StringLiteralNode(x.Info()) as ValueNode)
    );

  public static readonly TokenListParser<SuperpowerTokenType, ValueNode> NumberLiteralParser =
    Parse.Ref(
      () =>
        Token
          .EqualTo(SuperpowerTokenType.numberLiteral)
          .Select(x => new NumberLiteralNode(x.Info()) as ValueNode)
    );

  public static readonly TokenListParser<SuperpowerTokenType, ValueNode> LiteralParser = Parse.Ref(
    () => Parse.OneOf(StringLiteralParser, NumberLiteralParser)
  );

  // Symbol
  public static readonly TokenListParser<SuperpowerTokenType, SymbolNode> SymbolParser = Parse.Ref(
    () => Token.EqualTo(SuperpowerTokenType.symbol).Select(x => new SymbolNode(x.Info()))
  );

  // Spread
  public static readonly TokenListParser<SuperpowerTokenType, ValueNode> SpreadParser = Parse
    .Ref(() => Token.EqualTo(SuperpowerTokenType.spread))
    .IgnoreThen(SymbolParser)
    .WithSourceInfo()
    .Select((x) => new SpreadExpNode(x.sourceInfo, x.value) as ValueNode);

  // Helper Methods

  public static T ParseString<T>(string source, TokenListParser<SuperpowerTokenType, T> parser)
  {
    var tokens = SuperpowerTokenizer.tokenizer.Tokenize(source);
    return parser.AtEnd().Parse(tokens);
  }

  public static ProgramNode ParseString(string source)
  {
    var tokens = SuperpowerTokenizer.tokenizer.Tokenize(source);
    return ProgramParser.AtEnd().Parse(tokens);
  }

  public static TokenListParserResult<SuperpowerTokenType, ProgramNode> TryParse(string source)
  {
    var tokens = SuperpowerTokenizer.tokenizer.Tokenize(source);
    return ProgramParser.AtEnd().TryParse(tokens);
  }
}

public static class SuperpowerParserExtensions
{
  public static SourceCodeInfo Info(this Token<SuperpowerTokenType> token)
  {
    return new SourceCodeInfo(token.ToStringValue(), (token.Position.Absolute, token.Span.Length));
  }

  public static Func<Prev, TokenListParser<SuperpowerTokenType, (Prev, Curr)>> WithPrevious<
    Prev,
    Curr
  >(this TokenListParser<SuperpowerTokenType, Curr> source)
  {
    return (prev) => source.Select(curr => (prev, curr));
  }

  public static TokenListParser<TKind, (SourceCodeInfo sourceInfo, T value)> WithSourceInfo<
    TKind,
    T
  >(this TokenListParser<TKind, T> parser)
  {
    return WithSpan(parser).Select((x) => (SourceCodeInfo.FromSpan(x.span), x.value));
  }

  public static TokenListParser<TKind, ((int start, int length) span, T value)> WithSpan<TKind, T>(
    this TokenListParser<TKind, T> parser
  )
  {
    return delegate(TokenList<TKind> input)
    {
      var result = parser(input);
      if (result.HasValue)
      {
        var tokens = result.Location;
        var first = tokens.FirstOrDefault();
        var last = tokens.LastOrDefault();
        var start = first.Position.Absolute;
        var end = last.Position.Absolute + last.Span.Length;
        return TokenListParserResult.Value(
          ((start, end - start), result.Value),
          input,
          result.Remainder
        );
      }
      else
      {
        return TokenListParserResult.CastEmpty<TKind, T, ((int start, int length), T)>(result);
      }
    };
  }

  public static TokenListParser<TKind, T?> Optional<TKind, T>(this TokenListParser<TKind, T> source)
    where T : class
  {
    return source.Try().Or(Parse.Return<TKind, T>(null!))!;
  }

  public static TokenListParser<TKind, bool> Flag<TKind, T>(this TokenListParser<TKind, T> source)
  {
    return source.Select(x => true).Try().Or(Parse.Return<TKind, bool>(false));
  }

  public static TokenListParser<TKind, T> ThenIgnore<TKind, T, U>(
    this TokenListParser<TKind, T> source,
    TokenListParser<TKind, U> ignore
  )
  {
    return from output in source from ignored in ignore select output;
  }

  public static TokenListParser<TKind, T[]> SeparatedBy<TKind, T, U>(
    this TokenListParser<TKind, T> source,
    TokenListParser<TKind, U> separator
  )
  {
    return Parse.OneOf(
      from first in source
      from rest in separator.IgnoreThen(source).Many()
      select new T[] { first }.Push(rest),
      Parse.Return<TKind, T[]>([])
    );
  }
}
