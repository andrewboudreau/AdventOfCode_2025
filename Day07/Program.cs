// https://adventofcode.com/2025/day/7
// --- Day 7: Laboratories ---
//
// Tachyon beam simulation:
// - Beam enters at 'S' and moves downward
// - Empty space '.' allows beam to pass
// - Splitter '^' stops beam and emits two new beams (left and right)
// - Count total number of times a splitter is hit
var grid = new Grid<char>(ReadLines(), line => line);

// Find start position 'S'
var start = grid.First(n => n.Value == 'S');

// Track active beams as (x, y) positions moving downward
var beams = new Queue<(int x, int y)>();
beams.Enqueue((start.X, start.Y + 1)); // Start one below S

// Track which splitters have been hit (each splitter only counts once)
var hitSplitters = new HashSet<(int x, int y)>();

while (beams.Count > 0)
{
    var (x, y) = beams.Dequeue();

    // Check if beam is out of bounds
    if (grid[x, y] is not Node<char> node)
        continue;

    if (node.Value == '^')
    {
        // Beam hits splitter - only spawn new beams if not already hit
        if (hitSplitters.Add((x, y)))
        {
            beams.Enqueue((x - 1, y)); // Left beam continues down
            beams.Enqueue((x + 1, y)); // Right beam continues down
        }
        // Otherwise beam is absorbed by already-active splitter
    }
    else
    {
        // Empty space - beam continues downward
        beams.Enqueue((x, y + 1));
    }
}

int splitCount = hitSplitters.Count;
splitCount.ToConsole(x => $"Part 1: {x}");

// Part 2: Count distinct timelines (paths through the manifold)
// Each splitter branches into left OR right (many-worlds interpretation)
// Use memoization: at each (x, y), how many timelines reach the exit?

var memo = new Dictionary<(int x, int y), long>();

long CountTimelines(int x, int y)
{
    // Out of bounds = 1 complete timeline (particle exited)
    if (grid[x, y] is not Node<char> node)
        return 1;

    // Check memo
    if (memo.TryGetValue((x, y), out var cached))
        return cached;

    long result;
    if (node.Value == '^')
    {
        // Splitter: branch into left and right timelines
        result = CountTimelines(x - 1, y) + CountTimelines(x + 1, y);
    }
    else
    {
        // Empty space: continue downward
        result = CountTimelines(x, y + 1);
    }

    memo[(x, y)] = result;
    return result;
}

long part2 = CountTimelines(start.X, start.Y + 1);
part2.ToConsole(x => $"Part 2: {x}");
