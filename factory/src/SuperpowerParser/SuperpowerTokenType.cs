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
  letKeyword,

  // Literals
  stringLiteral,
  numberLiteral,

  // Symbol
  symbol,
  unknown,
}
