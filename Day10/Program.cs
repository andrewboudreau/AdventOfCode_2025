// https://adventofcode.com/2025/day/10
// --- Day 10: Factory ---

using System.Collections;

var machines = ReadLines().Select(Machine.Parse).ToList();

int total = machines.Sum(m => m.Solve(verbose: false));
total.ToConsole(x => $"Part 1: {x}");

int total2 = machines.Sum(m => m.SolvePart2(verbose: false));
total2.ToConsole(x => $"Part 2: {x}");

class Machine
{
    public bool[] Target { get; }
    public int[][] Buttons { get; }
    public int[] Joltage { get; }  // Part 2: target counter values

    // BitArray representations
    public BitArray TargetBits { get; }
    public BitArray[] ButtonBits { get; }

    public int LightCount => Target.Length;
    public int ButtonCount => Buttons.Length;
    public int CounterCount => Joltage.Length;  // Part 2

    public Machine(bool[] target, int[][] buttons, int[] joltage)
    {
        Target = target;
        Buttons = buttons;
        Joltage = joltage;

        // Create BitArray for target
        TargetBits = new BitArray(target);

        // Create BitArray for each button (which lights it toggles)
        ButtonBits = [.. buttons.Select(indices =>
        {
            var bits = new BitArray(target.Length);
            foreach (var idx in indices)
            {
                bits[idx] = true;
            }
            return bits;
        })];
    }

    public static Machine Parse(string line)
    {
        // Format: [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}

        // Extract target state from [...]
        var bracketStart = line.IndexOf('[');
        var bracketEnd = line.IndexOf(']');
        var targetStr = line[(bracketStart + 1)..bracketEnd];
        var target = targetStr.Select(c => c == '#').ToArray();

        // Extract joltage from {...}
        var braceStart = line.IndexOf('{');
        var braceEnd = line.IndexOf('}');
        var joltageStr = line[(braceStart + 1)..braceEnd];
        var joltage = joltageStr.Split(',').Select(int.Parse).ToArray();

        // Extract buttons from (...) groups
        var buttons = new List<int[]>();
        var remaining = line[(bracketEnd + 1)..braceStart];

        var i = 0;
        while (i < remaining.Length)
        {
            if (remaining[i] == '(')
            {
                var end = remaining.IndexOf(')', i);
                var content = remaining[(i + 1)..end];
                var indices = content.Split(',').Select(int.Parse).ToArray();
                buttons.Add(indices);
                i = end + 1;
            }
            else
            {
                i++;
            }
        }

        return new Machine(target, [.. buttons], joltage);
    }

    public override string ToString()
    {
        var targetStr = string.Join("", Target.Select(b => b ? '#' : '.'));
        var buttonsStr = string.Join(" ", Buttons.Select(b => $"({string.Join(",", b)})"));
        return $"[{targetStr}] {buttonsStr}";
    }

    public string ToBitString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Target: {BitStr(TargetBits)}");
        for (int i = 0; i < ButtonBits.Length; i++)
            sb.AppendLine($"  B{i}:   {BitStr(ButtonBits[i])}");
        return sb.ToString();
    }

    private static string BitStr(BitArray bits)
    {
        var chars = new char[bits.Length];
        for (int i = 0; i < bits.Length; i++)
            chars[i] = bits[i] ? '1' : '0';
        return new string(chars);
    }

    /// <summary>
    /// Solves for minimum button presses using Gaussian elimination over GF(2).
    ///
    /// CONCEPTUAL OVERVIEW:
    /// ====================
    /// We're solving: B0*x0 ⊕ B1*x1 ⊕ B2*x2 ⊕ ... = Target
    /// Where xi is 0 (don't press) or 1 (press button i)
    ///
    /// This is a system of linear equations, one per light:
    ///   Light 0: b0[0]*x0 ⊕ b1[0]*x1 ⊕ ... = target[0]
    ///   Light 1: b0[1]*x0 ⊕ b1[1]*x1 ⊕ ... = target[1]
    ///   ...
    ///
    /// MATRIX SETUP:
    /// =============
    /// We build an "augmented matrix" where:
    ///   - Rows = lights (one equation per light)
    ///   - Columns = buttons + 1 (the +1 is the target, the "answer" column)
    ///
    /// Example for Machine 1:
    ///        B0 B1 B2 B3 B4 B5 | Target
    /// Light0: 0  0  0  0  1  1 | 0
    /// Light1: 0  1  0  0  0  1 | 1
    /// Light2: 0  0  1  1  1  0 | 1
    /// Light3: 1  1  0  1  0  0 | 0
    ///
    /// GAUSSIAN ELIMINATION:
    /// =====================
    /// Goal: Transform matrix so each row has a "leading 1" (pivot) further right than row above.
    /// This is called "row echelon form".
    ///
    /// Allowed operations (that don't change the solution):
    ///   1. Swap two rows
    ///   2. XOR one row into another (in regular math: add rows; in GF(2): XOR)
    ///
    /// Algorithm:
    ///   For each column (button), find a row with 1 in that column.
    ///   Use it as "pivot" - XOR it into all other rows that have 1 in that column.
    ///   This "eliminates" that variable from other equations.
    /// </summary>
    public int Solve(bool verbose = true)
    {
        // STEP 1: Build augmented matrix
        // Rows = lights, Columns = buttons + target
        // We transpose our button data (buttons are columns, lights are rows)
        int numRows = LightCount;
        int numCols = ButtonCount + 1; // +1 for target column

        var matrix = new BitArray[numRows];
        for (int row = 0; row < numRows; row++)
        {
            matrix[row] = new BitArray(numCols);
            // Fill in button columns: does button j affect light row?
            for (int col = 0; col < ButtonCount; col++)
            {
                matrix[row][col] = ButtonBits[col][row];
            }
            // Last column is the target
            matrix[row][ButtonCount] = TargetBits[row];
        }

        if (verbose)
        {
            Console.WriteLine("Initial augmented matrix:");
            PrintMatrix(matrix, ButtonCount);
        }

        // STEP 2: Gaussian elimination (forward phase)
        // Goal: Get row echelon form - each row's leading 1 is to the right of the row above
        int pivotRow = 0;
        var pivotCols = new List<int>(); // Track which columns have pivots

        for (int col = 0; col < ButtonCount && pivotRow < numRows; col++)
        {
            // Find a row with 1 in this column (at or below current pivot row)
            int foundRow = -1;
            for (int row = pivotRow; row < numRows; row++)
            {
                if (matrix[row][col])
                {
                    foundRow = row;
                    break;
                }
            }

            if (foundRow == -1)
            {
                // No pivot in this column - this button is a "free variable"
                // (can be 0 or 1, doesn't matter for reachability)
                continue;
            }

            // Swap found row to pivot position
            if (foundRow != pivotRow)
            {
                (matrix[pivotRow], matrix[foundRow]) = (matrix[foundRow], matrix[pivotRow]);
            }

            // Eliminate: XOR this row into all OTHER rows that have 1 in this column
            for (int row = 0; row < numRows; row++)
            {
                if (row != pivotRow && matrix[row][col])
                {
                    matrix[row].Xor(matrix[pivotRow]);
                }
            }

            pivotCols.Add(col);
            pivotRow++;
        }

        if (verbose)
        {
            Console.WriteLine("\nAfter elimination (reduced row echelon form):");
            PrintMatrix(matrix, ButtonCount);
        }

        // STEP 3: Check for inconsistency
        // If any row is [0 0 0 ... 0 | 1], the system has no solution
        for (int row = 0; row < numRows; row++)
        {
            bool allZeroCoeffs = true;
            for (int col = 0; col < ButtonCount; col++)
            {
                if (matrix[row][col]) { allZeroCoeffs = false; break; }
            }
            if (allZeroCoeffs && matrix[row][ButtonCount])
            {
                if (verbose) Console.WriteLine("NO SOLUTION - inconsistent system");
                return -1;
            }
        }

        // STEP 4: Find FREE variables (columns without pivots)
        // These can be 0 or 1 - we'll try all combinations to find minimum presses
        var freeCols = Enumerable.Range(0, ButtonCount).Except(pivotCols).ToList();

        if (verbose)
        {
            Console.WriteLine($"\nPivot columns (determined): [{string.Join(", ", pivotCols)}]");
            Console.WriteLine($"Free columns (can be 0 or 1): [{string.Join(", ", freeCols)}]");
            Console.WriteLine($"Searching {1 << freeCols.Count} combinations of free variables...");
        }

        // STEP 5: Try all 2^k combinations of free variables, find minimum weight solution
        bool[]? bestSolution = null;
        int bestPresses = int.MaxValue;

        // Iterate through all 2^k combinations (k = number of free variables)
        int numCombinations = 1 << freeCols.Count; // 2^k
        for (int combo = 0; combo < numCombinations; combo++)
        {
            var solution = new bool[ButtonCount];

            // Set free variables based on bits of 'combo'
            for (int i = 0; i < freeCols.Count; i++)
            {
                solution[freeCols[i]] = (combo & (1 << i)) != 0;
            }

            // Back-substitute to find pivot variable values
            // For each pivot row, the equation is:
            //   x_pivot + (sum of free vars in that row) = target
            // So: x_pivot = target XOR (sum of free vars in that row)
            for (int i = 0; i < pivotCols.Count; i++)
            {
                int pivotCol = pivotCols[i];
                bool value = matrix[i][ButtonCount]; // Start with target value

                // XOR in the contribution from free variables
                foreach (int freeCol in freeCols)
                {
                    if (matrix[i][freeCol] && solution[freeCol])
                    {
                        value = !value; // XOR with 1
                    }
                }

                solution[pivotCol] = value;
            }

            // Count presses for this solution
            int presses = solution.Count(b => b);
            if (presses < bestPresses)
            {
                bestPresses = presses;
                bestSolution = (bool[])solution.Clone();
            }
        }

        if (verbose)
        {
            Console.WriteLine($"\nBest solution (buttons to press): {string.Join("", bestSolution!.Select(b => b ? "1" : "0"))}");
            Console.WriteLine($"Minimum button presses needed: {bestPresses}");

            // Verify solution
            var result = new BitArray(LightCount);
            for (int i = 0; i < ButtonCount; i++)
            {
                if (bestSolution![i])
                    result.Xor(ButtonBits[i]);
            }
            Console.WriteLine($"Verification - Result:  {BitStr(result)}");
            Console.WriteLine($"              Target:  {BitStr(TargetBits)}");
            Console.WriteLine($"              Match: {BitStr(result) == BitStr(TargetBits)}");
        }

        return bestPresses;
    }

    private void PrintMatrix(BitArray[] matrix, int buttonCount)
    {
        for (int row = 0; row < matrix.Length; row++)
        {
            var sb = new System.Text.StringBuilder("  ");
            for (int col = 0; col < matrix[row].Length; col++)
            {
                if (col == buttonCount) sb.Append("| ");
                sb.Append(matrix[row][col] ? "1 " : "0 ");
            }
            Console.WriteLine(sb.ToString());
        }
    }

    /// <summary>
    /// Part 2: Solve for minimum button presses to reach joltage targets.
    /// Uses long arithmetic and frequent normalization to avoid overflow.
    /// </summary>
    public int SolvePart2(bool verbose = true)
    {
        int numRows = CounterCount;
        int numCols = ButtonCount;

        // Build augmented matrix [A | b] using long to avoid overflow
        var matrix = new long[numRows, numCols + 1];

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                matrix[row, col] = Buttons[col].Contains(row) ? 1 : 0;
            }
            matrix[row, numCols] = Joltage[row];
        }

        if (verbose)
        {
            Console.WriteLine($"Part 2: Joltage targets = {{{string.Join(",", Joltage)}}}");
            Console.WriteLine("Initial augmented matrix:");
            PrintLongMatrix(matrix, numRows, numCols + 1, numCols);
        }

        // Gaussian elimination with normalization after each step
        var pivotCols = new List<int>();
        int pivotRow = 0;

        for (int col = 0; col < numCols && pivotRow < numRows; col++)
        {
            // Find row with smallest non-zero absolute value (for numerical stability)
            int foundRow = -1;
            long minAbs = long.MaxValue;
            for (int row = pivotRow; row < numRows; row++)
            {
                if (matrix[row, col] != 0 && Math.Abs(matrix[row, col]) < minAbs)
                {
                    minAbs = Math.Abs(matrix[row, col]);
                    foundRow = row;
                }
            }

            if (foundRow == -1) continue;

            // Swap rows
            if (foundRow != pivotRow)
            {
                for (int c = 0; c <= numCols; c++)
                    (matrix[pivotRow, c], matrix[foundRow, c]) = (matrix[foundRow, c], matrix[pivotRow, c]);
            }

            // Normalize pivot row immediately
            NormalizeRow(matrix, pivotRow, numCols);

            // Eliminate other rows
            for (int row = 0; row < numRows; row++)
            {
                if (row != pivotRow && matrix[row, col] != 0)
                {
                    long pivotVal = matrix[pivotRow, col];
                    long rowVal = matrix[row, col];
                    // Use LCM-based elimination to keep numbers smaller
                    long g = GCDLong(Math.Abs(pivotVal), Math.Abs(rowVal));
                    long multPivot = rowVal / g;
                    long multRow = pivotVal / g;
                    for (int c = 0; c <= numCols; c++)
                    {
                        matrix[row, c] = matrix[row, c] * multRow - matrix[pivotRow, c] * multPivot;
                    }
                    // Normalize this row after elimination
                    NormalizeRow(matrix, row, numCols);
                }
            }

            pivotCols.Add(col);
            pivotRow++;
        }

        if (verbose)
        {
            Console.WriteLine("\nAfter elimination:");
            PrintLongMatrix(matrix, numRows, numCols + 1, numCols);
        }

        // Check for inconsistency
        for (int row = 0; row < numRows; row++)
        {
            bool allZero = true;
            for (int col = 0; col < numCols; col++)
            {
                if (matrix[row, col] != 0) { allZero = false; break; }
            }
            if (allZero && matrix[row, numCols] != 0)
            {
                if (verbose) Console.WriteLine("No solution exists - inconsistent!");
                return -1;
            }
        }

        var freeCols = Enumerable.Range(0, numCols).Except(pivotCols).ToList();

        if (verbose)
        {
            Console.WriteLine($"Pivot columns: [{string.Join(", ", pivotCols)}]");
            Console.WriteLine($"Free columns: [{string.Join(", ", freeCols)}]");
        }

        // Calculate per-button bounds
        int sumTargets = Joltage.Sum();
        var buttonBounds = new int[numCols];
        for (int b = 0; b < numCols; b++)
        {
            buttonBounds[b] = Buttons[b].Length > 0 ? Buttons[b].Min(c => Joltage[c]) : sumTargets;
        }

        int bestPresses = int.MaxValue;
        long[]? bestSolution = null;

        var freeRanges = freeCols.Select(col => buttonBounds[col] + 1).ToArray();

        long totalCombosLong = 1;
        foreach (var range in freeRanges)
        {
            totalCombosLong *= range;
            if (totalCombosLong > 100_000_000) { totalCombosLong = -1; break; }
        }

        if (totalCombosLong < 0)
        {
            if (verbose) Console.WriteLine("Search space too large - using greedy approach");
            var solution = new long[numCols];
            bool valid = TryBackSubstitute(matrix, pivotCols, numCols, solution);
            if (valid)
            {
                bestPresses = (int)solution.Sum();
                bestSolution = solution;
            }
        }
        else
        {
            int totalCombos = (int)totalCombosLong;
            if (verbose && freeCols.Count > 0)
                Console.WriteLine($"Searching {totalCombos} combinations...");

            for (int combo = 0; combo < totalCombos; combo++)
            {
                var solution = new long[numCols];

                int temp = combo;
                for (int i = 0; i < freeCols.Count; i++)
                {
                    solution[freeCols[i]] = temp % freeRanges[i];
                    temp /= freeRanges[i];
                }

                if (TryBackSubstitute(matrix, pivotCols, numCols, solution))
                {
                    int presses = (int)solution.Sum();
                    if (presses < bestPresses)
                    {
                        bestPresses = presses;
                        bestSolution = (long[])solution.Clone();
                    }
                }
            }
        }

        if (bestSolution == null)
        {
            if (verbose) Console.WriteLine("No non-negative solution found!");
            return -1;
        }

        if (verbose)
        {
            Console.WriteLine($"\nBest solution: [{string.Join(", ", bestSolution)}]");
            Console.WriteLine($"Minimum presses: {bestPresses}");

            var result = new long[CounterCount];
            for (int b = 0; b < ButtonCount; b++)
            {
                foreach (int counter in Buttons[b])
                    result[counter] += bestSolution[b];
            }
            Console.WriteLine($"Verification: {{{string.Join(",", result)}}} == {{{string.Join(",", Joltage)}}} ? {result.SequenceEqual(Joltage.Select(j => (long)j))}");
        }

        return bestPresses;
    }

    private static bool TryBackSubstitute(long[,] matrix, List<int> pivotCols, int numCols, long[] solution)
    {
        for (int i = pivotCols.Count - 1; i >= 0; i--)
        {
            int pivotCol = pivotCols[i];
            long pivotVal = matrix[i, pivotCol];

            long rhs = matrix[i, numCols];
            for (int c = 0; c < numCols; c++)
            {
                if (c != pivotCol)
                    rhs -= matrix[i, c] * solution[c];
            }

            if (rhs % pivotVal != 0) return false;
            solution[pivotCol] = rhs / pivotVal;
            if (solution[pivotCol] < 0) return false;
        }
        return true;
    }

    private static void NormalizeRow(long[,] matrix, int row, int numCols)
    {
        long gcd = 0;
        for (int c = 0; c <= numCols; c++)
            gcd = GCDLong(gcd, Math.Abs(matrix[row, c]));
        if (gcd > 1)
        {
            for (int c = 0; c <= numCols; c++)
                matrix[row, c] /= gcd;
        }
        // Ensure leading non-zero is positive
        for (int c = 0; c <= numCols; c++)
        {
            if (matrix[row, c] != 0)
            {
                if (matrix[row, c] < 0)
                {
                    for (int cc = 0; cc <= numCols; cc++)
                        matrix[row, cc] = -matrix[row, cc];
                }
                break;
            }
        }
    }

    private static long GCDLong(long a, long b) => b == 0 ? a : GCDLong(b, a % b);

    private static int GCD(int a, int b) => b == 0 ? a : GCD(b, a % b);

    private void PrintLongMatrix(long[,] matrix, int rows, int cols, int separator)
    {
        for (int row = 0; row < rows; row++)
        {
            var sb = new System.Text.StringBuilder("  ");
            for (int col = 0; col < cols; col++)
            {
                if (col == separator) sb.Append("| ");
                sb.Append($"{matrix[row, col],4} ");
            }
            Console.WriteLine(sb.ToString());
        }
    }

    private void PrintIntMatrix(int[,] matrix, int rows, int cols, int separator)
    {
        for (int row = 0; row < rows; row++)
        {
            var sb = new System.Text.StringBuilder("  ");
            for (int col = 0; col < cols; col++)
            {
                if (col == separator) sb.Append("| ");
                sb.Append($"{matrix[row, col],3} ");
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
