namespace Factory;

public enum FactoryErrorType
{
  info,
  warning,
  error,
}

public class FactoryLanguageError(
  int lexonPosition,
  int lexonLength,
  FactoryErrorType errorType,
  string errorMessage
)
{
  public int lexonPosition { get; private set; } = lexonPosition;
  public int lexonLength { get; private set; } = lexonLength;
  public FactoryErrorType errorType { get; private set; } = errorType;
  public string errorMessage { get; private set; } = errorMessage;
}
