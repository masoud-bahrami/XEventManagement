namespace Quantum.ApplicationService;

public static class FunctionalExtensions
{
    public static async Task<T> Pipe<T>(this Task<T> task, Func<T, Task> func)
    {
        var input = await task;
        await func(input);
        return input;
    }

    public static async Task<TOut> Pipe<TIn, TOut>(this Task<TIn> task, Func<TIn, Task<TOut>> func)
        => await func(await task);

    public static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> func)
        => func(input);
}