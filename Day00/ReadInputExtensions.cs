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
}
