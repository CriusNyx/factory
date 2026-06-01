namespace Factory.Superpower;

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
  arrowOw,
  equalSign,
  semicolon,
  underscore,

  // Math Symbols

  plus,
  minus,
  asterisk,
  forwardSlash,
  percent,

  // Keywords
  recipeKeyword,
  resourceKeyword,
  lineKeyword,
  altKeyword,
  outKeyword,
  printKeyword,
  tallyKeyword,
  inlineKeyword,
  inKeyword,
  limitKeyword,
  letKeyword,
  import,

  // Literals
  stringLiteral,
  numberLiteral,

  // Symbol
  symbol,
  unknown,
}
