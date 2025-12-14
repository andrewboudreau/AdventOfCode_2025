// https://adventofcode.com/2025/day/11
// --- Day 11: Reactor ---

// Parse the graph: node -> list of neighbors (outgoing edges)
var graph = new Dictionary<string, List<string>>();
var inputLines = ReadLines().ToList();

foreach (var line in inputLines)
{
    var parts = line.Split(':');
    var node = parts[0].Trim();
    var neighbors = parts[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    graph[node] = neighbors;
}

// Ensure "out" exists in graph even if it has no outgoing edges
if (!graph.ContainsKey("out"))
    graph["out"] = [];

// Memoization caches
var memo1 = new Dictionary<string, long>();
var memo2 = new Dictionary<(string, bool, bool), long>();

// Count all paths from start to target using DFS with memoization
long CountPaths(string current, string target)
{
    if (current == target) return 1;
    if (!graph.TryGetValue(current, out var neighbors)) return 0;
    if (memo1.TryGetValue(current, out var cached)) return cached;

    long count = 0;
    foreach (var neighbor in neighbors)
    {
        count += CountPaths(neighbor, target);
    }
    return memo1[current] = count;
}

// Part 2: Count paths that visit BOTH required nodes (with memoization)
long CountPathsWithRequired(string current, string target, bool visitedDac, bool visitedFft)
{
    // Update visited state
    if (current == "dac") visitedDac = true;
    if (current == "fft") visitedFft = true;

    // Only count if we reached target AND visited both required nodes
    if (current == target)
        return (visitedDac && visitedFft) ? 1 : 0;

    if (!graph.TryGetValue(current, out var neighbors)) return 0;

    var key = (current, visitedDac, visitedFft);
    if (memo2.TryGetValue(key, out var cached)) return cached;

    long count = 0;
    foreach (var neighbor in neighbors)
    {
        count += CountPathsWithRequired(neighbor, target, visitedDac, visitedFft);
    }
    return memo2[key] = count;
}

var pathCount = CountPaths("you", "out");
var pathCount2 = CountPathsWithRequired("svr", "out", false, false);

Console.WriteLine($"Part 1: {pathCount}");
Console.WriteLine($"Part 2: {pathCount2}");

// Output graph for visualization
Console.WriteLine("\n--- Graph Data (paste into visualize.html sampleGraph) ---");
foreach (var line in inputLines)
{
    Console.WriteLine(line);
}
