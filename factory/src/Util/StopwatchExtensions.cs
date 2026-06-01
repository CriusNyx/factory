using System.Diagnostics;

namespace Factory.Util;

public static class StopwatchExtensions
{
  public static long Lap(this Stopwatch stopwatch)
  {
    var output = stopwatch.ElapsedMilliseconds;
    stopwatch.Reset();
    return output;
  }
}
