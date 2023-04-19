namespace Utils;

public static class Utils
{
    public const int MatrixSize = 512;
    private static readonly Random Random = new(1234);


    public static int[] GenerateVector(int length)
    {
        return Enumerable.Range(0, length).AsParallel()
            .Aggregate(new int[length], (array, i) =>
            {
                array[i] = Random.Next(0, 10);
                return array;
            }
            ).ToArray();
    }

    public static int[][] GenerateMatrix(int rows, int? columns = null)
    {
        columns ??= rows;

        return Enumerable.Range(0, rows).AsParallel()
            .Aggregate(
                new int[rows][],
                (matrix, i) =>
                {
                    matrix[i] = GenerateVector(columns.Value);
                    return matrix;
                });
    }

    public static void Print2DArray<T>(T[][] matrix)
    {
        foreach (var column in matrix)
        {
            foreach (var element in column)
            {
                Console.Write($"{element}\t");
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    public static int[][] SumMatrix(ref int[][] left, ref int[][] right)
    {
        var rows = left.Length;
        var cols = left[0].Length;

        var res = new int[rows].Select(_ => new int[cols]).ToArray();

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                res[i][j] = left[i][j] + right[i][j];
            }
        }

        return res;
    }

    public static bool Equal(ref int[][] left, ref int[][] right)
    {
        var rows = left.Length;
        var cols = left[0].Length;

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                if (left[i][j] != right[i][j])
                {
                    return false;
                }
            }
        }

        return true;
    }
}