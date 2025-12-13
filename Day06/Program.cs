// https://adventofcode.com/2025/day/6
// --- Day 6: Trash Compactor ---

// Read input once for both parts
var lines = ReadLines().ToList();
var grid = new Grid<char>(lines, line => line.PadRight(lines.Max(l => l.Length)));

// Part 1: Numbers are written in rows, with operators below each column
List<List<int>> columns = [];
List<char> operators = [];

// Parse by splitting on whitespace
foreach (var row in lines)
{
    foreach (var (ordinal, item) in row.Split(' ', StringSplitOptions.RemoveEmptyEntries).Index())
    {
        if (char.IsNumber(item[0]))
        {
            // make sure our pivot table is big enough
            if (columns.Count <= ordinal)
            {
                columns.Add([]);
            }

            // add to pivot table
            columns[ordinal].Add(int.Parse(item));
        }
        else
        {
            // add to list of operators
            operators.Add(item[0]);
        }
    }
}

// Process
long total = 0;
foreach (var (ordinal, op) in operators.Index())
{
    if (op == '+')
    {
        total += columns[ordinal].Sum();
    }
    else
    {
        total += columns[ordinal].Product();
    }
}

total.ToConsole(t => $"Part 1: {t}");

// Part 2: Numbers are written in columns (top=most significant, bottom=least significant)
// Read right-to-left, one column at a time, grouping by problems separated by space columns.
//
// Example: "64 " column-wise with operator '+' below
//   Col 0: '6','2','3' -> 623  (reading digits top-to-bottom forms the number)
//   Col 1: '4','3','1' -> 431
//   Col 2: ' ',' ','4' -> 4    (leading spaces ignored)
// Problem: 4 + 431 + 623 = 1058

// Part 2: Parse column by column, right to left
// Each column of digits (top to bottom) forms one number
// Space-only columns separate problems

var problems = new List<(char op, List<long> numbers)>();
var currentNumbers = new List<long>();
char? currentOp = null;

// Process columns right-to-left
for (int col = grid.Width - 1; col >= 0; col--)
{
    // Get operator (bottom row of this column)
    char op = grid[col, grid.Height - 1]!.Value;

    // Get digits from this column using Slice (excludes operator row)
    var columnChars = grid.Slice(x: col, y: ..^1)
        .Select(n => n.Value)
        .ToList();

    // Check if this column has any digits
    bool hasDigits = columnChars.Any(char.IsDigit);

    if (hasDigits)
    {
        // Build number using Aggregate: multiply by 10, add digit
        // Skip leading non-digits AND take only digits
        long number = columnChars
            .SkipWhile(c => !char.IsDigit(c))
            .TakeWhile(char.IsDigit)
            .Aggregate(0L, (acc, c) => acc * 10 + (c - '0'));

        currentNumbers.Add(number);

        // Track operator (should be same for all columns in a problem, or space)
        if (op != ' ')
            currentOp = op;
    }
    else if (currentNumbers.Count > 0)
    {
        // Space column = end of current problem
        if (currentOp.HasValue)
        {
            problems.Add((currentOp.Value, currentNumbers));
        }
        currentNumbers = [];
        currentOp = null;
    }
}

// Don't forget the last problem
if (currentNumbers.Count > 0 && currentOp.HasValue)
{
    problems.Add((currentOp.Value, currentNumbers));
}

// Calculate each problem and sum
long part2 = problems.Sum(p =>
{
    return p.op switch
    {
        '+' => p.numbers.Sum(),
        '*' => p.numbers.Product(),
        _ => throw new InvalidOperationException($"Unknown operator: {p.op}")
    };
});

part2.ToConsole(x => $"Part 2: {x}");

