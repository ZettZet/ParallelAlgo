const int numOfThreads = 5;

Enumerable.Range(0, numOfThreads).AsParallel().WithDegreeOfParallelism(numOfThreads)
    .ForAll(i => Console.WriteLine($"Hello, world of concurrency. From thread {i}; ThreadID: {Environment.CurrentManagedThreadId}"));