using System.Diagnostics;
using System.Reflection;

public class ProfileAttribute(string name) : Attribute
{
  public readonly string name = name;
}

public static class Profiler
{
  public static void Profile(string profileName, int timeout)
  {
    foreach (var method in typeof(Profiler).GetMethods())
    {
      var methodProfile = method.GetCustomAttribute<ProfileAttribute>()?.name;
      if (methodProfile == profileName)
      {
        RunProfile(profileName, () => method.Invoke(null, []), timeout, true);
        for (int i = 0; i < 5; i++)
        {
          RunProfile(profileName, () => method.Invoke(null, []), timeout, false);
        }
      }
    }
  }

  private static void RunProfile(string name, Action action, int timeout, bool dry)
  {
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    int count = 0;
    while (stopwatch.ElapsedMilliseconds < timeout)
    {
      action();
      count++;
    }
    if (!dry)
    {
      Console.WriteLine($"Profile {name} completed {count} iterations in {timeout} milliseconds");
    }
  }

  [Profile(nameof(AutocompleteSpeedTest))]
  public static void AutocompleteSpeedTest()
  {
    AutocompleteCache.Search("a");
  }
}
