public class Result<T>(bool hasValue, T value)
{
  public bool HasValue = hasValue;
  public T Value => value;
}

public static class Result
{
  public static Result<T> Succ<T>(T value)
  {
    return new Result<T>(true, value);
  }

  public static Result<T> Fail<T>()
  {
    return new Result<T>(false, default!);
  }
}
