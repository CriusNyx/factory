using Factory.Util;

namespace Factory.Superpower;

public static class ASTBuilder
{
  public static SourceCodeInfo Source(string sourceCode)
  {
    return new SourceCodeInfo(sourceCode, (-1, -1));
  }

  public static SymbolNode Sym(string symbol)
  {
    return new SymbolNode(Source(symbol));
  }

  public static StringLiteralNode StrLit(string lit)
  {
    return new StringLiteralNode(Source($"\"{lit}\""));
  }

  public static NumberLiteralNode NumLit(string lit)
  {
    return new NumberLiteralNode(Source(lit));
  }

  public static SpreadExpNode Spread(string symbol)
  {
    return new SpreadExpNode(Source(symbol), Sym(symbol));
  }

  public static ExpChainNode Chain(
    string symbol,
    params Func<ExpChainNode?, ExpChainNode>[] builderFuncs
  )
  {
    var cont = builderFuncs.Reverse().Aggregate(null as ExpChainNode, (curr, func) => func(curr));
    return new ExpChainNode(Source(""), Sym(symbol), cont!);
  }

  public static Func<ExpChainNode?, ExpChainNode> Deref(string symbol)
  {
    return (prev) => new ExpChainNode(Source(""), new DerefNode(Source(""), Sym(symbol)), prev!);
  }

  public static Func<ExpChainNode?, ExpChainNode> Invoke(params ValueNode[] arguments)
  {
    return (prev) => new ExpChainNode(Source(""), new InvocationNode(Source(""), arguments), prev!);
  }

  public static MathExpNode MathExp(ValueNode leading, params TermChainNode[] chain)
  {
    return new MathExpNode(Source(""), leading, chain);
  }

  public static TermChainNode TermChain(string op, ValueNode value)
  {
    return new TermChainNode(
      Source(""),
      SuperpowerParser.ParseString(op, SuperpowerParser.TermOperatorParser),
      value
    );
  }

  public static TermNode Term(ValueNode start, params FactorChainNode[] chain)
  {
    return new TermNode(Source(""), start, chain);
  }

  public static FactorChainNode FactorChain(string op, ValueNode value)
  {
    return new FactorChainNode(
      Source(""),
      SuperpowerParser.ParseString(op, SuperpowerParser.FactorOperatorParser),
      value
    );
  }

  public static FactorNode Factor(ValueNode value, bool negative = false)
  {
    return FactorNode.Create(Source(""), negative, value);
  }

  public static ParentheticalUnit Paren(ValueNode value)
  {
    return new ParentheticalUnit(Source(""), value);
  }

  public static AssignExpNode Assign(ExpChainNode lhs, ValueNode rhs)
  {
    return AssignExpNode.Create(Source(""), lhs, rhs);
  }

  public static PrintStatementNode Print(params ValueNode[] values)
  {
    return PrintStatementNode.Create(Source(""), values);
  }

  public static OutExpNode Out(params string[] symbols)
  {
    return new OutExpNode(Source(""), symbols.Map(Sym));
  }

  public static InExpNode In(params string[] symbols)
  {
    return new InExpNode(Source(""), symbols.Map(Sym));
  }

  public static AltExpNode Alt(params string[] symbols)
  {
    return new AltExpNode(Source(""), symbols.Map(Sym));
  }

  public static TallyExpNode Tally(bool inline, params string[] symbols)
  {
    return new TallyExpNode(Source(""), inline, symbols.Map(Sym));
  }

  public static LimitExpNode Limit(params LimitValueExpNode[] values)
  {
    return new LimitExpNode(Source(""), values);
  }

  public static LimitValueExpNode LimitVal(decimal value, string symbol)
  {
    return new LimitValueExpNode(Source(""), NumLit(value.ToString()), Sym(symbol));
  }

  public static LineNode Line(string name, params LineExpNode[] expressions)
  {
    return new LineNode(Source(""), Sym(name), expressions);
  }

  public static RecipeNode Recipe(
    bool alt,
    string name,
    QuantityNode[] input,
    QuantityNode[] output
  )
  {
    return new RecipeNode(Source(""), alt, Sym(name), input, output);
  }

  public static QuantityNode Quantity(decimal number, string name)
  {
    return new QuantityNode(Source(""), NumLit(number.ToString()), Sym(name));
  }

  public static ResourceNode Resource(string name)
  {
    return new ResourceNode(Source(""), Sym(name));
  }

  public static ImportNode Import(string importPath)
  {
    return new ImportNode(Source(""), StrLit(importPath));
  }

  public static ProgramNode Program(params StatementNode[] statements)
  {
    return new ProgramNode(Source(""), statements);
  }

  public static EquivalentIfExists EquivalentIfExists()
  {
    return new EquivalentIfExists();
  }
}
