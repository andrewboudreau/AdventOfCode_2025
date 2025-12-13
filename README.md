# AdventOfCode 2025
[2025 Advent of Code](https://adventofcode.com) Solutions in C#

## Build & Run

```bash
# Build entire solution
dotnet build AdventOfCode_2025.sln

# Run a specific day
dotnet run --project Day01 -- Day01/problem.txt
```

## Project Structure

| Folder | Description |
|--------|-------------|
| Day00  | Shared utilities library |
| Day01+ | Daily puzzle solutions |


## Solutions

| Day | Puzzle | Solution |
|-----|--------|----------|
| 1 | [Secret Entrance](https://adventofcode.com/2025/day/1) | [Day01](Day01/) |
| 2 | [Gift Shop](https://adventofcode.com/2025/day/2) | [Day02](Day02/) |
| 3 | [Lobby](https://adventofcode.com/2025/day/3) | [Day03](Day03/) |
| 4 | [Printing Department](https://adventofcode.com/2025/day/4) | [Day04](Day04/) |
| 5 | [Cafeteria](https://adventofcode.com/2025/day/5) | [Day05](Day05/) |
| 6 | [Trash Compactor](https://adventofcode.com/2025/day/6) | [Day06](Day06/) |
| 7 | [Laboratories](https://adventofcode.com/2025/day/7) | [Day07](Day07/) |
| 8 | [Playground](https://adventofcode.com/2025/day/8) | [Day08](Day08/) |

## Visualizations

### Day 4 - Printing Department
Final grid state after removing all accessible paper rolls (white = remaining rolls, black = empty):

![Day 4 Output](Day04/output.bmp)

## Day00 Utilities

| File | Description |
|------|-------------|
| ReadInputs.cs | Span-based input parsing from files or stdin |
| ReadInputExtensions.cs | Convenience methods for reading integers, records |
| Grid.cs | 2D grid with neighbor traversal, BFS, region detection |
| GridRenderExtensions.cs | Grid rendering including bitmap export |
| Node.cs | Grid node with position, value, distance, visited state |
| SimulationGrid.cs | Sparse grid for simulations with viewport rendering |
| Graph.cs | Generic graph with pathfinding |
| Line.cs | 2D line segment with path enumeration |
| CircularRange.cs | Circular number range with zero-crossing tracking |
| CircularEnumerable.cs | Infinitely repeating sequence |
| EnumerableExtensions.cs | Collection helpers (SplitToInts, Second, Third, etc.) |
| CombinatoricsEnumerableExtensions.cs | Permutations, combinations |
| StringParsingExtensions.cs | ParseInt, ParseIntegers, ParseLongs |
| ArrayExtensions.cs | Array utilities |
| RenderExtensions.cs | Fluent .ToConsole() for output |
| Cards.cs | Playing card utilities |
| EnumerableTuples.cs | Tuple enumeration helpers |
