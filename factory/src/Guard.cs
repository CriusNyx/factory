public static class Guard
{
  public static void GuardLength<T>(this T[] self, T[] other, string? message = null)
  {
    if (self.Length != other.Length)
    {
      throw new InvalidOperationException(
        message ?? "Arrays have different lengths but are expected to have the same."
      );
    }
  }

  public static void GuardLength<T>(this T[] self, T[] other, Func<Exception> createException)
  {
    if (self.Length != other.Length)
    {
      throw createException();
    }
  }
}
