// https://adventofcode.com/2025/day/8
// --- Day 8: Playground ---
//
// PROBLEM: Given N boxes in 3D space, make exactly 1000 connections between boxes,
// prioritizing shortest distances. Calculate the product of the three largest resulting
// connected components (circuits).
//
// ALGORITHM: Kruskal's MST (Minimum Spanning Tree/Forest) using Union-Find
// - Time: O(E log E) for sorting pairs + O(E α(N)) for union operations ≈ O(E log E)
// - Space: O(E) for pairs + O(N) for union-find structures
// where E = N(N-1)/2 pairwise distances, α(N) is inverse Ackermann (effectively constant)

var boxes = ReadLines()
    .Select(line => line.Split(',').Select(int.Parse).ToArray())
    .Select(p => (X: p[0], Y: p[1], Z: p[2]))
    .ToArray();

// PHASE 1: Build complete graph - all pairwise Euclidean distances
// For N boxes, generates N(N-1)/2 edges. Each edge represents a potential connection.
var pairs = new List<(int i, int j, double dist)>();
for (int i = 0; i < boxes.Length; i++)
{
    for (int j = i + 1; j < boxes.Length; j++)
    {
        var dx = (double)boxes[i].X - boxes[j].X;
        var dy = (double)boxes[i].Y - boxes[j].Y;
        var dz = (double)boxes[i].Z - boxes[j].Z;
        var distSquared = dx * dx + dy * dy + dz * dz;  // Use squared distance; sqrt is unnecessary for comparisons
        pairs.Add((i, j, distSquared));
    }
}

// PHASE 2: Sort edges by weight (distance) - foundation of Kruskal's algorithm
// Greedy approach: always try to connect nearest unconnected components first
pairs.Sort((a, b) => a.dist.CompareTo(b.dist));

// PHASE 3: Union-Find (Disjoint Set Union) data structure
// Maintains a forest of trees where each tree represents a connected component.
// Two key optimizations make this nearly O(1) per operation:
//
//// 1. PATH COMPRESSION (in Find): Flattens tree during traversal
////    Example: A→B→C→D becomes A→D, B→D, C→D after Find(A)
////
//// 2. UNION BY RANK (in Union): Attaches smaller tree under larger tree root
////    Keeps tree depth logarithmic, preventing degenerate chains
////
//// Together, these give amortized O(α(N)) per operation where α is inverse Ackermann function.
//// For all practical N, α(N) ≤ 4, making this effectively constant time.

var parent = Enumerable.Range(0, boxes.Length).ToArray();  // Initially, each node is its own parent (own set)
var rank = new int[boxes.Length];  // Approximate tree depth for union-by-rank heuristic

// Find the root representative of a set (with path compression)
// Path compression: During traversal, point every node directly to root
int Find(int x)
{
    if (parent[x] != x)
        parent[x] = Find(parent[x]);  // Recursively find root and compress path
    return parent[x];
}

// Merge two sets (union by rank)
// Union by rank: Attach smaller tree under root of larger tree to minimize depth
// Returns true if a merge occurred, false if already in same set
bool Union(int x, int y)
{
    var px = Find(x);
    var py = Find(y);
    if (px == py) return false;  // Already in same component - no merge

    // Attach smaller rank tree under larger rank tree
    if (rank[px] < rank[py])
        parent[px] = py;
    else if (rank[px] > rank[py])
        parent[py] = px;
    else
    {
        parent[py] = px;
        rank[px]++;  // Only increment rank when trees of equal rank merge
    }
    return true;  // Merge occurred
}

// PHASE 4: Kruskal's algorithm - greedily add shortest edges that don't form cycles
// Union-Find efficiently detects cycles: if Find(i) == Find(j), they're already connected
// Track: circuit count and the final merge that connects everything (for Part 2)
var circuitCount = boxes.Length;  // Start with N separate circuits
(int i, int j, double dist) lastMergePair = default;

var connectionsToMake = Math.Min(1000, pairs.Count);
for (int i = 0; i < connectionsToMake; i++)
{
    if (Union(pairs[i].i, pairs[i].j))
    {
        circuitCount--;
        lastMergePair = pairs[i];
    }
}

// PHASE 5: Analyze resulting connected components for Part 1
// After 1000 connections, boxes are partitioned into disjoint circuits.
var circuitSizes = boxes.Select((_, i) => Find(i))  // Map each box to its circuit root
    .GroupBy(root => root)                           // Group by circuit
    .Select(g => g.Count())                          // Count boxes per circuit
    .OrderByDescending(x => x)                       // Largest circuits first
    .ToList();

// ANSWER Part 1: Product of three largest circuit sizes
var result = circuitSizes.Take(3).Aggregate(1L, (acc, x) => acc * x);
result.ToConsole(x => $"Part 1: {x}");

// PHASE 6: Part 2 - Continue until all boxes form a single circuit (if not already)
for (int i = connectionsToMake; i < pairs.Count && circuitCount > 1; i++)
{
    if (Union(pairs[i].i, pairs[i].j))
    {
        circuitCount--;
        lastMergePair = pairs[i];
    }
}

// ANSWER Part 2: Product of X coordinates of the final connecting pair
var part2 = (long)boxes[lastMergePair.i].X * boxes[lastMergePair.j].X;
part2.ToConsole(x => $"Part 2: {x}");
