var boxes = ReadLines()
    .Select(line => line.Split(',').Select(int.Parse).ToArray())
    .Select(p => (X: p[0], Y: p[1], Z: p[2]))
    .ToArray();

// Calculate all pairwise distances
var pairs = new List<(int i, int j, double dist)>();
for (int i = 0; i < boxes.Length; i++)
{
    for (int j = i + 1; j < boxes.Length; j++)
    {
        var dx = boxes[i].X - boxes[j].X;
        var dy = boxes[i].Y - boxes[j].Y;
        var dz = boxes[i].Z - boxes[j].Z;
        var dist = Math.Sqrt(dx * dx + dy * dy + dz * dz);
        pairs.Add((i, j, dist));
    }
}

// Sort by distance
pairs.Sort((a, b) => a.dist.CompareTo(b.dist));

// Union-Find
var parent = Enumerable.Range(0, boxes.Length).ToArray();
var rank = new int[boxes.Length];

int Find(int x)
{
    if (parent[x] != x)
        parent[x] = Find(parent[x]);
    return parent[x];
}

void Union(int x, int y)
{
    var px = Find(x);
    var py = Find(y);
    if (px == py) return;

    if (rank[px] < rank[py])
        parent[px] = py;
    else if (rank[px] > rank[py])
        parent[py] = px;
    else
    {
        parent[py] = px;
        rank[px]++;
    }
}

// Make the 1000 shortest connections
var connectionsToMake = Math.Min(1000, pairs.Count);
for (int i = 0; i < connectionsToMake; i++)
{
    Union(pairs[i].i, pairs[i].j);
}

// Count circuit sizes
var circuitSizes = boxes.Select((_, i) => Find(i))
    .GroupBy(root => root)
    .Select(g => g.Count())
    .OrderByDescending(x => x)
    .ToList();

var result = circuitSizes.Take(3).Aggregate(1L, (acc, x) => acc * x);

result.ToConsole(x => $"Part 1: {x}");
