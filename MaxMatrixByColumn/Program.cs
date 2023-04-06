using Benchmarker;
using static Utils.Utils;

Console.OutputEncoding = System.Text.Encoding.Unicode;

var matrix = GenerateMatrix(MatrixSize);

void Calculate(int threads) {
    Enumerable.Range(0, matrix.Length).AsParallel().WithDegreeOfParallelism(threads)
        .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
        .Select(i => matrix[i].Max()).Sum();
}

var bSingle = new BenchingSingleThreaded(Calculate);

Console.WriteLine($"{bSingle}\n");
foreach (var numOfThreads in Enumerable.Range(1, 4).Select(i => (int)Math.Pow(2, i))) {
    var bMultiple = new BenchingMultiThreaded(Calculate, numOfThreads, bSingle.Elapsed);
    Console.WriteLine($"{bMultiple}\n");
}