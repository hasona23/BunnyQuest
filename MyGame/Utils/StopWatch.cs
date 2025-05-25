using System;
using System.Diagnostics;

namespace MyGame.Utils;
internal class StopWatch : IDisposable
{
    private Stopwatch _stopWatch;
    private string _process;
    public StopWatch(string process)
    {
        _stopWatch = new();
        _stopWatch.Start();
        _process = process;
    }
    public void Dispose()
    {
        _stopWatch.Stop();
        Console.WriteLine($"Process: {_process} => {_stopWatch.ElapsedMilliseconds}");

    }
}
