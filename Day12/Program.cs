// https://adventofcode.com/2025/day/12
// --- Day 12: Christmas Tree Farm ---

// Parse input - shapes are separated by blank lines, then regions are individual lines
var shapes = new List<HashSet<(int r, int c)>>();
var regions = new List<(int W, int H, int[] Counts)>();
var currentCells = new HashSet<(int r, int c)>();
int row = 0;

foreach (var line in ReadAllLines())
{
    if (string.IsNullOrEmpty(line))
    {
        if (currentCells.Count > 0)
        {
            shapes.Add(currentCells);
            currentCells = [];
            row = 0;
        }
    }
    else if (line.Contains('x') && line.Contains(':'))
    {
        // Region line like "4x4: 0 0 0 0 2 0"
        var parts = line.Split(':');
        var dims = parts[0].Split('x');
        var counts = parts[1].Trim().Split(' ').Select(int.Parse).ToArray();
        regions.Add((int.Parse(dims[0]), int.Parse(dims[1]), counts));
    }
    else if (line.EndsWith(':'))
    {
        // Shape index line like "0:" - reset for new shape
        row = 0;
    }
    else
    {
        // Shape pattern line with '#' and '.'
        for (int c = 0; c < line.Length; c++)
            if (line[c] == '#')
                currentCells.Add((row, c));
        row++;
    }
}

// Generate all variants (rotations + flips) for each shape
var allVariants = shapes.Select(shape =>
{
    var seen = new HashSet<string>();
    var variants = new List<HashSet<(int r, int c)>>();
    var current = shape;

    for (int flip = 0; flip < 2; flip++)
    {
        for (int rot = 0; rot < 4; rot++)
        {
            var norm = Normalize(current);
            var key = string.Join(",", norm.OrderBy(p => p.r).ThenBy(p => p.c).Select(p => $"{p.r},{p.c}"));
            if (seen.Add(key))
                variants.Add(norm);
            current = Rotate90(current);
        }
        current = FlipH(shape);
    }
    return variants;
}).ToList();

// Solve each region
int canFit = 0;
foreach (var region in regions)
{
    var shapesToPlace = region.Counts
        .SelectMany((count, idx) => Enumerable.Repeat(idx, count))
        .ToList();

    int totalCells = shapesToPlace.Sum(idx => shapes[idx].Count);
    if (totalCells > region.W * region.H)
        continue;

    var grid = new bool[region.H, region.W];
    if (TryPack(grid, shapesToPlace, 0))
        canFit++;
}

Console.WriteLine($"Part 1: {canFit}");

// Helper functions
HashSet<(int r, int c)> Normalize(HashSet<(int r, int c)> s)
{
    int minR = s.Min(p => p.r), minC = s.Min(p => p.c);
    return [.. s.Select(p => (p.r - minR, p.c - minC))];
}

HashSet<(int r, int c)> Rotate90(HashSet<(int r, int c)> s) =>
    [.. s.Select(p => (p.c, -p.r))];

HashSet<(int r, int c)> FlipH(HashSet<(int r, int c)> s) =>
    [.. s.Select(p => (p.r, -p.c))];

bool TryPack(bool[,] grid, List<int> toPlace, int idx, bool debug = false)
{
    if (idx >= toPlace.Count) return true;

    int h = grid.GetLength(0), w = grid.GetLength(1);

    // Try all possible positions for each variant
    foreach (var variant in allVariants[toPlace[idx]])
    {
        // Try all base positions
        for (int br = 0; br < h; br++)
        {
            for (int bc = 0; bc < w; bc++)
            {
                bool valid = variant.All(p =>
                {
                    int r = br + p.r, c = bc + p.c;
                    return r >= 0 && r < h && c >= 0 && c < w && !grid[r, c];
                });

                if (valid)
                {
                    foreach (var p in variant) grid[br + p.r, bc + p.c] = true;
                    if (TryPack(grid, toPlace, idx + 1, debug)) return true;
                    foreach (var p in variant) grid[br + p.r, bc + p.c] = false;
                }
            }
        }
    }
    return false;
}
