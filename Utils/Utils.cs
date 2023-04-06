namespace Utils;

public static class Utils {
    public const int MatrixSize = 10000;

    public static int[] GenerateVector(int length) {
        return Enumerable.Range(0, length).AsParallel()
            .Aggregate(new int[length], (array, i) => {
                    array[i] = new Random().Next(0, 10);
                    return array;
                }
            ).ToArray();
    }

    public static int[][] GenerateMatrix(int rows, int? columns = null) {
        columns ??= rows;

        return Enumerable.Range(0, rows).AsParallel()
            .Aggregate(
                new int[rows][],
                (matrix, i) => {
                    matrix[i] = GenerateVector(columns.Value);
                    return matrix;
                });
    }

    public static void Print2DArray<T>(T[][] matrix) {
        foreach (var column in matrix) {
            foreach (var element in column) {
                Console.Write($"{element}\t");
            }

            Console.WriteLine();
        }
    }
}