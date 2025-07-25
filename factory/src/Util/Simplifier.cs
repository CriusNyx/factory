public interface Simplifier<T>
{
  public bool TrySimplify(out T? value);
}

public static class SimplifierExtensions
{
  public static T Simplify<T>(this T simplifier)
    where T : Simplifier<T>
  {
    T output = simplifier;

    while (output!.TrySimplify(out var temp))
    {
      output = temp!;
    }
    return output;
  }
}
