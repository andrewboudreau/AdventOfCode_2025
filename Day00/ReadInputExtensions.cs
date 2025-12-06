namespace Day00;

/// <summary>
/// Extension and convenience methods for input reading, built on top of the core ReadInputs methods.
/// </summary>
public static class ReadInputExtensions
{
    /// <summary>
    /// Reads lines, splits each on spaces, parses as integers, and transforms with factory.
    /// </summary>
    /// <remarks>
    /// Input:
    /// <code>
    /// 1 2 3
    /// 4 5 6
    /// </code>
    /// Usage:
    /// <code>
    /// // Sum each row
    /// var rowSums = Read(ints =&gt; ints.Sum());
    /// // rowSums: [6, 15]
    /// </code>
    /// </remarks>
    public static IEnumerable<T> Read<T>(Func<IEnumerable<int>, T> factory)
        => ReadInputs.ReadLines()
            .TakeWhile(x => !string.IsNullOrEmpty(x))
            .Select(line => factory(
                line!
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(int.Parse)));

    /// <summary>
    /// Reads multi-line records separated by blank lines.
    /// </summary>
    /// <remarks>
    /// Input:
    /// <code>
    /// name: Alice
    /// age: 30
    ///
    /// name: Bob
    /// age: 25
    /// </code>
    /// Usage:
    /// <code>
    /// var names = ReadRecords(lines =&gt; lines[0].Split(": ")[1]);
    /// // names: ["Alice", "Bob"]
    /// </code>
    /// </remarks>
    public static IEnumerable<T> ReadRecords<T>(Func<string[], T> factory)
    {
        var records = new List<string>();
        foreach (var row in ReadInputs.ReadLines())
        {
            if (string.IsNullOrEmpty(row))
            {
                yield return factory([.. records]);
                records.Clear();
            }
            else
            {
                records.Add(row!);
            }
        }

        if (records.Count > 0)
        {
            yield return factory([.. records]);
        }
    }

    /// <summary>
    /// Reads lines as integers (one integer per line).
    /// </summary>
    /// <remarks>
    /// Input:
    /// <code>
    /// 42
    /// 17
    /// 99
    /// </code>
    /// Usage:
    /// <code>
    /// var total = ReadIntegers().Sum();
    /// // total: 158
    /// </code>
    /// </remarks>
    public static IEnumerable<int> ReadIntegers()
        => ReadInputs.Read<int>(line => int.Parse(line));

    /// <summary>
    /// Converts each character in the string to its numeric value.
    /// </summary>
    /// <remarks>
    /// <code>
    /// var digits = AsIntegers("123");
    /// // digits: [1, 2, 3]
    /// </code>
    /// </remarks>
    public static IEnumerable<int> AsIntegers(string source)
        => source.Select(c => (int)char.GetNumericValue(c));

    /// <summary>
    /// Reads input as rows of single-digit integers.
    /// </summary>
    /// <remarks>
    /// Input:
    /// <code>
    /// 123
    /// 456
    /// </code>
    /// Usage:
    /// <code>
    /// var grid = ReadAsRowsOfIntegers();
    /// // grid: [[1,2,3], [4,5,6]]
    /// </code>
    /// </remarks>
    public static IEnumerable<IEnumerable<int>> ReadAsRowsOfIntegers()
        => ReadInputs.Read(line => AsIntegers(line.ToString()));

    /// <summary>
    /// Reads two sections of input separated by a blank line.
    /// Part 1 lines are processed to create a context, then part 2 lines are processed with that context.
    /// </summary>
    /// <remarks>
    /// Input:
    /// <code>
    /// 47|53
    /// 97|13
    ///
    /// 75,47,61,53,29
    /// 97,61,53,29,13
    /// </code>
    /// Usage:
    /// <code>
    /// var result = ReadParts(
    ///     lines => lines.Select(ParseRule).ToHashSet(),
    ///     (lines, rules) => lines.Count(line => IsValid(line, rules)));
    /// </code>
    /// </remarks>
    public static TResult ReadParts<TContext, TResult>(
        Func<IEnumerable<string>, TContext> arrange,
        Func<IEnumerable<string>, TContext, TResult> execute)
    {
        using var enumerator = ReadInputs.ReadLines().GetEnumerator();

        IEnumerable<string> YieldUntilBlank()
        {
            while (enumerator.MoveNext() && !string.IsNullOrEmpty(enumerator.Current))
            {
                yield return enumerator.Current;
            }
        }

        var context = arrange(YieldUntilBlank());
        return execute(YieldUntilBlank(), context);
    }

    /// <summary>
    /// Parses space-delimited integers from a span into a destination span.
    /// </summary>
    /// <remarks>
    /// <code>
    /// Span&lt;int&gt; nums = stackalloc int[3];
    /// ParseIntegers("10 20 30", nums);
    /// // nums: [10, 20, 30]
    /// </code>
    /// </remarks>
    public static void ParseIntegers(ReadOnlySpan<char> line, Span<int> destination, char delimiter = ' ')
    {
        int index = 0;
        int start = 0;

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == delimiter || char.IsWhiteSpace(line[i]))
            {
                if (i > start)
                {
                    destination[index++] = int.Parse(line[start..i]);
                }
                start = i + 1;
            }
        }

        if (start < line.Length)
        {
            destination[index] = int.Parse(line[start..]);
        }
    }

    private const StringSplitOptions DefaultSplitOptions =
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    private const char DefaultSeparator = ' ';

    /// <summary>
    /// Reads all lines, splits each by the specified separator, and processes via a single action.
    /// Minimizes allocations by yielding split segments lazily.
    /// Defaults to space separator with RemoveEmptyEntries | TrimEntries.
    /// </summary>
    /// <remarks>
    /// Input:
    /// <code>
    /// 123 328  51 64
    ///  45 64  387 23
    /// *   +   *   +
    /// </code>
    /// Usage:
    /// <code>
    /// ReadSplit(rows =>
    /// {
    ///     foreach (var row in rows)
    ///     {
    ///         foreach (var item in row)
    ///             Console.Write($"[{item}] ");
    ///         Console.WriteLine();
    ///     }
    /// });
    /// </code>
    /// </remarks>
    public static void ReadSplit(
        Action<IEnumerable<IEnumerable<string>>> action,
        char separator = DefaultSeparator,
        StringSplitOptions options = DefaultSplitOptions)
    {
        action(ReadSplitLines(separator, options));
    }

    /// <summary>
    /// Reads all lines, splits each by the specified separator, and transforms via factory.
    /// Defaults to space separator with RemoveEmptyEntries | TrimEntries.
    /// </summary>
    public static T ReadSplit<T>(
        Func<IEnumerable<IEnumerable<string>>, T> factory,
        char separator = DefaultSeparator,
        StringSplitOptions options = DefaultSplitOptions)
    {
        return factory(ReadSplitLines(separator, options));
    }

    private static IEnumerable<IEnumerable<string>> ReadSplitLines(char separator, StringSplitOptions options)
    {
        foreach (var line in ReadInputs.ReadLines())
        {
            if (line == null)
                yield break;

            yield return SplitLine(line, separator, options);
        }
    }

    private static IEnumerable<string> SplitLine(string line, char separator, StringSplitOptions options)
    {
        int start = 0;
        bool removeEmpty = (options & StringSplitOptions.RemoveEmptyEntries) != 0;
        bool trim = (options & StringSplitOptions.TrimEntries) != 0;

        for (int i = 0; i <= line.Length; i++)
        {
            if (i == line.Length || line[i] == separator)
            {
                var segment = line.AsSpan(start, i - start);

                if (trim)
                    segment = segment.Trim();

                if (!removeEmpty || segment.Length > 0)
                    yield return segment.ToString();

                start = i + 1;
            }
        }
    }
}
