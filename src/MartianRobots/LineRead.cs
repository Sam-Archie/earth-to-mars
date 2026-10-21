namespace MartianRobots;

/// <summary>What a line reader made of one line from Earth: either the parsed value, or a hint naming the shape it expects.</summary>
public sealed class LineRead<T> where T : class
{
    private LineRead(T? value, string? hint) => (Value, Hint) = (value, hint);

    public T? Value { get; }

    public string? Hint { get; }

    internal static LineRead<T> Parsed(T value) => new(value, null);

    internal static LineRead<T> Bad(string hint) => new(null, hint);
}
