namespace Day00;

/// <summary>
/// Core input reading methods. All methods are span-based for performance.
/// Input source is determined by command-line args: file path if provided, otherwise stdin.
/// </summary>
public static class ReadInputs
{
    /// <summary>
    /// Gets the input file path from command-line arguments, or empty string if reading from stdin.
    /// </summary>
    private static string GetInputFile()
    {
        var args = Environment.GetCommandLineArgs();
        return args.Length > 1 ? args[1] : string.Empty;
    }

    /// <summary>
    /// Core span-based read with callback. Processes each line as a <see cref="ReadOnlySpan{char}"/>.
    /// </summary>
    public static void Read(ReadOnlySpanAction<char> action)
    {
        var inputFile = GetInputFile();

        if (!string.IsNullOrEmpty(inputFile))
        {
            foreach (var line in File.ReadLines(inputFile))
            {                
				action(line.AsSpan());
            }
        }
        else
        {
            string? line;
            while ((line = Console.ReadLine()) != null)
            {
                action(line.AsSpan());
            }
        }
    }

    /// <summary>
    /// Core span-based read with factory. Transforms each line using the provided factory function.
    /// Stops at empty lines.
    /// </summary>
    public static IEnumerable<T> Read<T>(Func<ReadOnlySpan<char>, T> factory)
    {
        var inputFile = GetInputFile();

        if (!string.IsNullOrEmpty(inputFile))
        {
            foreach (var line in File.ReadLines(inputFile))
            {
                if (string.IsNullOrEmpty(line))
                    break;

                yield return factory(line.AsSpan());
            }
        }
        else
        {
            string? line;
            while ((line = Console.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line))
                    break;

                yield return factory(line.AsSpan());
            }
        }
    }

    /// <summary>
    /// Reads all lines as strings, including empty lines. Useful for compatibility and multi-pass scenarios.
    /// </summary>
    public static IEnumerable<string?> ReadAllLines()
    {
        var inputFile = GetInputFile();

        if (!string.IsNullOrEmpty(inputFile))
        {
            foreach (var line in File.ReadAllLines(inputFile))
            {
                yield return line;
            }
        }
        else
        {
            string? line;
            while ((line = Console.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }

    /// <summary>
    /// Reads all non-empty lines as strings. Skips null or whitespace-only lines.
    /// </summary>
    public static IEnumerable<string> ReadLines()
    {
        var inputFile = GetInputFile();

        if (!string.IsNullOrEmpty(inputFile))
        {
            foreach (var line in File.ReadAllLines(inputFile))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    yield return line;
            }
        }
        else
        {
            string? line;
            while ((line = Console.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    yield return line;
            }
        }
    }
}

public delegate void ReadOnlySpanAction<T>(ReadOnlySpan<T> span);
