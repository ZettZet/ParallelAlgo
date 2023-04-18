const int numOfThreads = 5;

Console.OutputEncoding = System.Text.Encoding.Unicode;

Enumerable.Range(0, numOfThreads).AsParallel().WithDegreeOfParallelism(numOfThreads)
    .ForAll(i => Console.WriteLine($"Привет, мир конкуренции. Поток номер {i}; ThreadID: {Environment.CurrentManagedThreadId}"));