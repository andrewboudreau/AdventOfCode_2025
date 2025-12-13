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
        var dx = boxes[i].X - boxes[j].X;
        var dy = boxes[i].Y - boxes[j].Y;
        var dz = boxes[i].Z - boxes[j].Z;
        var dist = Math.Sqrt(dx * dx + dy * dy + dz * dz);
        pairs.Add((i, j, dist));
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
void Union(int x, int y)
{
    var px = Find(x);
    var py = Find(y);
    if (px == py) return;  // Already in same component - skip to avoid cycle

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
}

// PHASE 4: Kruskal's algorithm - greedily add shortest edges that don't form cycles
// Union-Find efficiently detects cycles: if Find(i) == Find(j), they're already connected
// Result: Minimum Spanning Forest (MSF) with up to 1000 edges
var connectionsToMake = Math.Min(1000, pairs.Count);
for (int i = 0; i < connectionsToMake; i++)
{
    Union(pairs[i].i, pairs[i].j);  // Union is idempotent - redundant edges are no-ops
}

// PHASE 5: Analyze resulting connected components
// After 1000 unions, boxes are partitioned into disjoint circuits.
// Find() normalizes each box to its circuit's root representative.
var circuitSizes = boxes.Select((_, i) => Find(i))  // Map each box to its circuit root
    .GroupBy(root => root)                           // Group by circuit
    .Select(g => g.Count())                          // Count boxes per circuit
    .OrderByDescending(x => x)                       // Largest circuits first
    .ToList();

// ANSWER: Product of three largest circuit sizes
// Edge case handling: If fewer than 3 circuits exist, Take() returns what's available
Console.WriteLine($"DEBUG: Circuit sizes (top 10): {string.Join(", ", circuitSizes.Take(10))}");
Console.WriteLine($"DEBUG: Total circuits: {circuitSizes.Count}");
var result = circuitSizes.Aggregate(1L, (acc, x) => acc * x);

result.ToConsole(x => $"Part 1: {x}");
 
//589 too low
