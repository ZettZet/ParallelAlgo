using Benchmarker;
using static Utils.Utils;

Console.OutputEncoding = System.Text.Encoding.Unicode;

int[][] Strassen(ref int[][] left, ref int[][] right, int numOfTreads = 1) {
    var bSize = left.Length;

    if (bSize < 16) {
        return MultiplyBlockMatrices(ref left, ref right, bSize, 0, 0, 0, 0, bSize);
    }

    var halfSize = bSize / 2;
    var a_11 = GetBlockFromMatrix(ref left, halfSize, 0, 0);
    var a_22 = GetBlockFromMatrix(ref left, halfSize, 1, 1);
    var a_12 = GetBlockFromMatrix(ref left, halfSize, 0, 1);
    var a_21 = GetBlockFromMatrix(ref left, halfSize, 1, 0);

    var b_11 = GetBlockFromMatrix(ref right, halfSize, 0, 0);
    var b_22 = GetBlockFromMatrix(ref right, halfSize, 1, 1);
    var b_12 = GetBlockFromMatrix(ref right, halfSize, 0, 1);
    var b_21 = GetBlockFromMatrix(ref right, halfSize, 1, 0);

    int[][] d = Array.Empty<int[]>(),
        d_1 = Array.Empty<int[]>(),
        d_2 = Array.Empty<int[]>(),
        h_1 = Array.Empty<int[]>(),
        h_2 = Array.Empty<int[]>(),
        v_1 = Array.Empty<int[]>(),
        v_2 = Array.Empty<int[]>();

    Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = numOfTreads }, () => {
        var t1 = SumMatrix(ref a_11, ref a_22);
        var t2 = SumMatrix(ref b_11, ref b_22);
        d = Strassen(ref t1, ref t2);
    }, () => {
        var t3 = SubtractMatrix(ref a_12, ref a_22);
        var t4 = SumMatrix(ref b_21, ref b_22);
        d_1 = Strassen(ref t3, ref t4);
    }, () => {
        var t5 = SubtractMatrix(ref a_21, ref a_11);
        var t6 = SumMatrix(ref b_11, ref b_12);
        d_2 = Strassen(ref t5, ref t6);
    }, () => {
        var t7 = SumMatrix(ref a_11, ref a_12);
        h_1 = Strassen(ref t7, ref b_22);
    }, () => {
        var t8 = SumMatrix(ref a_21, ref a_22);
        h_2 = Strassen(ref t8, ref b_11);
    }, () => {
        var t9 = SubtractMatrix(ref b_21, ref b_11);
        v_1 = Strassen(ref a_22, ref t9);
    }, () => {
        var t10 = SubtractMatrix(ref b_12, ref b_22);
        v_2 = Strassen(ref a_11, ref t10);
    });

    var result = new int[bSize].Select((i) => new int[bSize]).ToArray();

    var res11 = SumMatrix(ref d, ref d_1);
    res11 = SumMatrix(ref res11, ref v_1);
    res11 = SubtractMatrix(ref res11, ref h_1);

    var res_12 = SumMatrix(ref v_2, ref h_1);

    var res_21 = SumMatrix(ref v_1, ref h_2);

    var res_22 = SumMatrix(ref d, ref d_2);
    res_22 = SumMatrix(ref res_22, ref v_2);
    res_22 = SubtractMatrix(ref res_22, ref h_2);

    result = PutBlockInMatrix(ref res11, ref result, 0, 0);
    result = PutBlockInMatrix(ref res_22, ref result, 1, 1);
    result = PutBlockInMatrix(ref res_12, ref result, 0, 1);
    result = PutBlockInMatrix(ref res_21, ref result, 1, 0);

    return result;
}

const int testMatrixSize = 128;
int[][] leftTest = GenerateMatrix(testMatrixSize);
int[][] rightTest = GenerateMatrix(testMatrixSize);


var strassen = Strassen(ref leftTest, ref rightTest, 8);
var cannon = MultiplyBlockMatrices(ref leftTest, ref rightTest, leftTest.Length, 0, 0, 0, 0, leftTest.Length);

Console.WriteLine(Equal(ref strassen, ref cannon));



int[][] left = GenerateMatrix(MatrixSize);
int[][] right = GenerateMatrix(MatrixSize);

void Calculate(int threads) {
    Strassen(ref left, ref right, threads);
}

var bSingle = new BenchingSingleThreaded(Calculate);
var temp = bSingle.Elapsed;

Console.WriteLine($"{bSingle}\n");
foreach (var numOfThreads in Enumerable.Range(1, 3).Select(i => 2 << i)) {
    var bMultiple = new BenchingMultiThreaded(Calculate, numOfThreads, temp);
    Console.WriteLine($"{bMultiple}\n");
}