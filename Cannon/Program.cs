using Benchmarker;
using static Utils.Utils;

Console.OutputEncoding = System.Text.Encoding.Unicode;

int[][] left = GenerateMatrix(MatrixSize);
int[][] right = GenerateMatrix(MatrixSize);

void Calculate(int threads)
{
    var blockInDimension = 4; // √(p)
    var (blockSize, _) = Math.DivRem(left.Length, blockInDimension); // n/√(p)


    var toCheck = Enumerable.Range(0, blockInDimension) //blockSize)
        .AsParallel()
        .WithDegreeOfParallelism(threads)
        .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
        .SelectMany(k => Enumerable.Range(0, blockInDimension)
            .SelectMany(row => Enumerable.Range(0, blockInDimension)
                .Select(col =>
                {
                    var index = (row + col + k) % blockInDimension;
                    return MultiplyBlockMatrices(ref left, ref right, blockSize, row, index, index, col);
                })
            )
        )
        .Aggregate(new int[left.Length].Select(_ => new int[left[0].Length]).ToArray(),
            (acc, elem) => SumMatrix(ref acc, ref elem));

    // Console.WriteLine(Equal(ref result, ref toCheck));
}

var bSingle = new BenchingSingleThreaded(Calculate);
var temp = bSingle.Elapsed;

Console.WriteLine($"{bSingle}\n");
foreach (var numOfThreads in Enumerable.Range(1, 3).Select(i => 2 << i))
{
    var bMultiple = new BenchingMultiThreaded(Calculate, numOfThreads, temp);
    Console.WriteLine($"{bMultiple}\n");
}