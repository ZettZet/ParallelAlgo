using System.Diagnostics;

namespace Benchmarker;

public record BenchingSingleThreaded(Action<int> Calculations) {
    private TimeSpan? _elapsed;
    public TimeSpan Elapsed => _elapsed ??= Invoke();
    
    private TimeSpan Invoke() {
        var timer = Stopwatch.StartNew();

        Calculations(1);

        timer.Stop();

        return timer.Elapsed;
    }

    public override string ToString() =>
        $"""
Кол-во потоков              : 1
Общее время выполнения      : {Elapsed.TotalSeconds:0.###} секунд
""";
}