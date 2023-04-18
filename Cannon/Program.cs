using Benchmarker;
using static Utils.Utils;

Console.OutputEncoding = System.Text.Encoding.Unicode;

int[][] MultiplyBlockMatrices(ref int[][] left,
    ref int[][] right,
    int bSize,
    int leftBlockRow,
    int leftBlockColumn,
    int rightBlockRow,
    int rightBlockColumn) {
    var rows = left.Length;
    var cols = left[0].Length;

    var res = new int[rows].Select(_ => new int[cols]).ToArray();
    var leftIInitial = leftBlockRow * bSize;
    var leftJInitial = leftBlockColumn * bSize;
    var rightIInitial = rightBlockRow * bSize;
    var rightJInitial = rightBlockColumn * bSize;

    for (var i = 0; i < bSize; i++) {
        for (var j = 0; j < bSize; j++) {
            for (var k = 0; k < bSize; k++) {
                res[leftIInitial + i][rightJInitial + j] += left[leftIInitial + i][leftJInitial + k] *
                                                            right[rightIInitial + k][rightJInitial + j];
            }
        }
    }

    return res;
}


int[][] left = GenerateMatrix(MatrixSize);
int[][] right = GenerateMatrix(MatrixSize);

// var result = MultiplyBlockMatrices(ref left, ref right, left.Length, 0, 0, 0, 0);

void Calculate(int threads) {
    var blockInDimension = threads == 1 ? 1 : 4; // √(p)
    var (blockSize, _) = Math.DivRem(left.Length, blockInDimension); // n/√(p)


    var toCheck = Enumerable.Range(0, blockInDimension) //blockSize)
        .AsParallel()
        .WithDegreeOfParallelism(threads)
        .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
        .SelectMany(k => Enumerable.Range(0, blockInDimension)
            .SelectMany(row => Enumerable.Range(0, blockInDimension)
                .Select(col => {
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

Console.WriteLine($"{bSingle}\n");
foreach (var numOfThreads in Enumerable.Range(1, 3).Select(i => 2 << i)) {
    var bMultiple = new BenchingMultiThreaded(Calculate, numOfThreads, bSingle.Elapsed);
    Console.WriteLine($"{bMultiple}\n");
}