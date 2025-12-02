namespace Day00;

/// <summary>
/// Represents a 2D line segment between two points.
/// </summary>
/// <remarks>
/// <code>
/// // Parse from string "1,2 -> 9,2"
/// var line = Line.Create("1,2 -> 9,2");
///
/// // Create directly
/// var line = new Line(1, 2, 9, 2);
///
/// // Iterate all points on the line
/// foreach (var (x, y) in line.Path())
///     Console.WriteLine($"{x},{y}");
/// </code>
/// </remarks>
/// <param name="x1">X coordinate of the first point.</param>
/// <param name="y1">Y coordinate of the first point.</param>
/// <param name="x2">X coordinate of the second point.</param>
/// <param name="y2">Y coordinate of the second point.</param>
public class Line(int x1, int y1, int x2, int y2)
{
    /// <summary>
    /// Creates a horizontal line at Y=0.
    /// </summary>
    /// <param name="x1">Starting X coordinate.</param>
    /// <param name="x2">Ending X coordinate.</param>
    public Line(int x1, int x2)
        : this(x1, 0, x2, 0)
    {
    }

    /// <summary>
    /// Creates a line from an array of coordinates [x1, y1, x2, y2].
    /// </summary>
    /// <param name="coords">Array of exactly 4 integers.</param>
    public Line(params int[] coords)
        : this(coords[0], coords[1], coords[2], coords[3])
    {
    }

    /// <summary>X coordinate of the first point.</summary>
    public int X1 { get; } = x1;

    /// <summary>Y coordinate of the first point.</summary>
    public int Y1 { get; } = y1;

    /// <summary>X coordinate of the second point.</summary>
    public int X2 { get; } = x2;

    /// <summary>Y coordinate of the second point.</summary>
    public int Y2 { get; } = y2;

    /// <summary>Minimum X value between the two points.</summary>
    public int MinX => Math.Min(X1, X2);

    /// <summary>Minimum Y value between the two points.</summary>
    public int MinY => Math.Min(Y1, Y2);

    /// <summary>Maximum X value between the two points.</summary>
    public int MaxX => Math.Max(X1, X2);

    /// <summary>Maximum Y value between the two points.</summary>
    public int MaxY => Math.Max(Y1, Y2);

    /// <summary>True if the line is horizontal (Y1 == Y2).</summary>
    public bool Horizontal => Y1 == Y2;

    /// <summary>True if the line is vertical (X1 == X2).</summary>
    public bool Vertical => X1 == X2;

    /// <summary>True if the line is diagonal (neither horizontal nor vertical).</summary>
    public bool Diagonal => !Horizontal && !Vertical;

    /// <summary>
    /// Enumerates all integer points along the line from (X1,Y1) to (X2,Y2).
    /// </summary>
    /// <remarks>
    /// Works for horizontal, vertical, and 45-degree diagonal lines.
    /// Uses step increments of -1, 0, or 1 in each direction.
    /// </remarks>
    /// <returns>Each (X, Y) coordinate on the path, inclusive of endpoints.</returns>
    public IEnumerable<(int X, int Y)> Path()
    {
        var current = (X1, Y1);
        while (current != (X2, Y2))
        {
            yield return current;
            current = (
                current.X1 + Math.Sign(X2 - X1),
                current.Y1 + Math.Sign(Y2 - Y1));
        }

        yield return current;
    }

    public override string ToString()
        => $"{X1},{Y1} -> {X2},{Y2}";

    /// <summary>
    /// Splits things like "1,2 -> 9,2" into two x.y points.
    /// </summary>
    /// <param name="input">the string input</param>
    /// <param name="points">The separator for the points</param>
    /// <param name="coords">The separator for the coordinates</param>
    /// <returns>an integer line from the input string</returns>
    public static Line Create(string input, string points, char coords)
        => new(input.Split(points, StringSplitOptions.TrimEntries).SplitToInts(coords).ToArray());

    public static Line Create(string input)
        => Create(input, "->", ',');
}
