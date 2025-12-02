namespace Day00;

/// <summary>
/// A circular range of integers with configurable size that tracks zero crossings.
/// Uses modular arithmetic for wraparound behavior.
/// </summary>
/// <remarks>
/// <code>
/// var range = new CircularRange();      // 0-99, starts at 50
/// range.MoveBackward(68);               // Position: 82, crossed 0 once
/// range.MoveForward(48);                // Position: 30, didn't cross 0
/// Console.WriteLine(range.CrossedZero);   // Total times crossed 0
/// Console.WriteLine(range.StoppedOnZero); // Times stopped exactly on 0
/// </code>
/// </remarks>
public class CircularRange
{
    /// <summary>
    /// Number of positions in the range (e.g., 100 for 0-99).
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Current position (0 to Size-1).
    /// </summary>
    public int Position { get; private set; }

    /// <summary>
    /// Total number of times position has been 0 during any movement.
    /// </summary>
    public int CrossedZero { get; private set; }

    /// <summary>
    /// Number of times a movement has ended at position 0.
    /// </summary>
    public int StoppedOnZero { get; private set; }

    /// <summary>
    /// Creates a new circular range.
    /// </summary>
    /// <param name="size">Number of positions (default 100 for 0-99).</param>
    /// <param name="startPosition">Initial position (default 50).</param>
    public CircularRange(int size = 100, int startPosition = 50)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be positive.");
        if (startPosition < 0 || startPosition >= size)
            throw new ArgumentOutOfRangeException(nameof(startPosition), "Start position must be within range.");

        Size = size;
        Position = startPosition;
    }

    /// <summary>
    /// Moves backward (toward lower numbers) by the specified distance.
    /// </summary>
    /// <param name="distance">Number of steps to move (must be positive).</param>
    /// <returns>Number of times zero was crossed during this movement.</returns>
    /// <remarks>
    /// When moving backward, we hit 0 at specific intervals:
    /// <list type="bullet">
    ///   <item>From position 5, moving back: we hit 0 at step 5, then 105, 205, etc.</item>
    ///   <item>From position 0, moving back: first step goes to 99, we hit 0 at step 100, 200, etc.</item>
    /// </list>
    /// </remarks>
    public int MoveBackward(int distance)
    {
        if (distance <= 0)
            throw new ArgumentOutOfRangeException(nameof(distance), "Distance must be positive.");

        int crossings;

        if (Position == 0)
        {
            // Starting at 0, first step moves to Size-1 (away from 0).
            // We only hit 0 again after a full cycle (Size steps).
            // Crossings occur at: Size, 2*Size, 3*Size, ...
            crossings = distance / Size;
        }
        else if (distance >= Position)
        {
            // We will reach 0 at least once.
            // First crossing at exactly 'Position' steps (e.g., from 5, hit 0 after 5 steps).
            // Additional crossings every 'Size' steps after that.
            // Formula: 1 (first crossing) + (remaining distance) / Size
            crossings = (distance - Position) / Size + 1;
        }
        else
        {
            // Distance is less than current position, we won't reach 0.
            // Example: at position 50, moving back 30 lands on 20.
            crossings = 0;
        }

        CrossedZero += crossings;
        Position = ((Position - distance) % Size + Size) % Size;

        // C# modulo with negative numbers keeps the sign:
        // Explanation of Position calculation:
        // Position = ((Position - distance) % Size + Size) % Size;
        //             └──────────┬────────┘
        //                        │
        //             Could be negative (e.g., 50 - 68 = -18)

        // Position = ((Position - distance) % Size + Size) % Size;
        //             └────────────┬──────────────┘
        //                          │
        //             Still could be negative (-18 % 100 = -18)

        // Position = ((Position - distance) % Size + Size) % Size;
        //             └──────────────────┬─────────────────┘
        //                                │
        //             Now positive (-18 + 100 = 82)

        // Position = ((Position - distance) % Size + Size) % Size;
        //             └────────────────────────┬────────────────────┘
        //                                      │
        //             Final mod in case original was positive
        //             (e.g., if step 3 gave us 182, this gives 82)

        if (Position == 0)
            StoppedOnZero++;

        return crossings;
    }

    /// <summary>
    /// Moves forward (toward higher numbers) by the specified distance.
    /// </summary>
    /// <param name="distance">Number of steps to move (must be positive).</param>
    /// <returns>Number of times zero was crossed during this movement.</returns>
    public int MoveForward(int distance)
    {
        if (distance <= 0)
            throw new ArgumentOutOfRangeException(nameof(distance), "Distance must be positive.");

        // Calculate crossings: we hit 0 when we wrap past Size-1
        int crossings = (Position + distance) / Size;

        CrossedZero += crossings;
        Position = (Position + distance) % Size;

        if (Position == 0)
            StoppedOnZero++;

        return crossings;
    }

    /// <summary>
    /// Resets to initial state.
    /// </summary>
    /// <param name="position">Position to reset to (default 50).</param>
    public void Reset(int position = 50)
    {
        if (position < 0 || position >= Size)
            throw new ArgumentOutOfRangeException(nameof(position), "Position must be within range.");

        Position = position;
        CrossedZero = 0;
        StoppedOnZero = 0;
    }

    public override string ToString() => $"CircularRange[{Position}/{Size - 1}] Crossed:{CrossedZero} Stopped:{StoppedOnZero}";
}
