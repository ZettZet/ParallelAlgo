using System.Diagnostics;

namespace Benchmarker;

public record BenchingMultiThreaded(Action<int> Calculations, int NumOfThreads, TimeSpan SingleThreaded) {
    private TimeSpan? _elapsed;
    public TimeSpan Elapsed => _elapsed ??= Invoke();
    public double SpeedUp => Elapsed.TotalSeconds/ SingleThreaded.TotalSeconds;
    public double Effectiveness => SpeedUp / NumOfThreads;
    private TimeSpan Invoke() {
        var timer = Stopwatch.StartNew();

        Calculations(NumOfThreads);

        timer.Stop();

        return timer.Elapsed;
    }

    public override string ToString() =>
        $"""
Кол-во потоков              : {NumOfThreads}
Общее время выполнения      : {Elapsed.TotalSeconds:0.###} секунд
Ускорение:                  : {SpeedUp:0.###}
Эффективность:              : {Effectiveness:0.###}
""";
}