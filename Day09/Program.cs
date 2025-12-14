// https://adventofcode.com/2025/day/9
// --- Day 9: Movie Theater ---
//
// Part 1: Find largest rectangle using any two red tiles as opposite corners
// Part 2: Rectangle must only contain red/green tiles (inside the polygon formed by red tiles)
//
// ALGORITHM:
// - Part 1: O(n²) check all pairs
// - Part 2: Coordinate compression + scanline to mark polygon interior,
//           then check if rectangle cells are all inside

var tiles = ReadLines()
    .Select(line => line.Split(',').Select(int.Parse).ToArray())
    .Select(p => (X: p[0], Y: p[1]))
    .ToArray();

// PART 1: Find maximum rectangle area using any two tiles as opposite corners
var maxArea = 0L;
for (int i = 0; i < tiles.Length; i++)
{
    for (int j = i + 1; j < tiles.Length; j++)
    {
        var width = Math.Abs(tiles[j].X - tiles[i].X) + 1;
        var height = Math.Abs(tiles[j].Y - tiles[i].Y) + 1;
        var area = (long)width * height;
        maxArea = Math.Max(maxArea, area);
    }
}
maxArea.ToConsole(x => $"Part 1: {x}");

// PART 2: Rectangle must be fully inside the polygon (red + green tiles only)
// The red tiles form vertices of a rectilinear polygon, connected by green tile edges.
// Interior of the polygon is also green.

// Coordinate compression: map unique X/Y values to indices
var xs = tiles.Select(t => t.X).Distinct().OrderBy(x => x).ToList();
var ys = tiles.Select(t => t.Y).Distinct().OrderBy(y => y).ToList();
var xIndex = xs.Select((x, i) => (x, i)).ToDictionary(p => p.x, p => p.i);
var yIndex = ys.Select((y, i) => (y, i)).ToDictionary(p => p.y, p => p.i);

// Grid cells: cell (i,j) represents region (xs[i], xs[i+1]) x (ys[j], ys[j+1])
int cellsX = xs.Count - 1;
int cellsY = ys.Count - 1;
var isInside = new bool[cellsX, cellsY];

// Build vertical edges of the polygon (for scanline algorithm)
var verticalEdges = new List<(int x, int yMin, int yMax)>();
for (int i = 0; i < tiles.Length; i++)
{
    var t1 = tiles[i];
    var t2 = tiles[(i + 1) % tiles.Length];
    if (t1.X == t2.X)  // Vertical edge
    {
        verticalEdges.Add((t1.X, Math.Min(t1.Y, t2.Y), Math.Max(t1.Y, t2.Y)));
    }
}

// Scanline: mark cells inside the polygon using even-odd rule
for (int j = 0; j < cellsY; j++)
{
    // Use midpoint of cell row for crossing detection
    int yMid = (ys[j] + ys[j + 1]) / 2;

    // Find vertical edges that cross this y-level (strictly)
    var crossings = verticalEdges
        .Where(e => e.yMin < yMid && yMid < e.yMax)
        .Select(e => e.x)
        .OrderBy(x => x)
        .ToList();

    // Inside between pairs of crossings (even-odd rule)
    for (int k = 0; k < crossings.Count - 1; k += 2)
    {
        int x1 = crossings[k];
        int x2 = crossings[k + 1];
        int i1 = xIndex[x1];
        int i2 = xIndex[x2];
        for (int ii = i1; ii < i2; ii++)
        {
            isInside[ii, j] = true;
        }
    }
}

// Check all pairs of red tiles for valid rectangles
long maxArea2 = 0;
for (int a = 0; a < tiles.Length; a++)
{
    for (int b = a + 1; b < tiles.Length; b++)
    {
        int minX = Math.Min(tiles[a].X, tiles[b].X);
        int maxX = Math.Max(tiles[a].X, tiles[b].X);
        int minY = Math.Min(tiles[a].Y, tiles[b].Y);
        int maxY = Math.Max(tiles[a].Y, tiles[b].Y);

        int iMin = xIndex[minX];
        int iMax = xIndex[maxX];
        int jMin = yIndex[minY];
        int jMax = yIndex[maxY];

        // Check if all cells in rectangle are inside polygon
        bool valid = true;
        for (int i = iMin; i < iMax && valid; i++)
        {
            for (int j = jMin; j < jMax && valid; j++)
            {
                if (!isInside[i, j])
                    valid = false;
            }
        }

        if (valid)
        {
            long width = maxX - minX + 1;
            long height = maxY - minY + 1;
            maxArea2 = Math.Max(maxArea2, width * height);
        }
    }
}
maxArea2.ToConsole(x => $"Part 2: {x}");
