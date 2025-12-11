# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an Advent of Code 2025 solutions repository implemented in C# targeting .NET 10.

## Build and Run Commands

```bash
# Build the entire solution
dotnet build AdventOfCode_2025.sln

# Build a specific day
dotnet build Day01/Day01.csproj

# Run a specific day's solution with input file
dotnet run --project Day01/Day01.csproj -- Day01/problem.txt

# Run with sample input
dotnet run --project Day01/Day01.csproj -- Day01/sample.txt

# Run interactively (reads from stdin)
dotnet run --project Day01/Day01.csproj
```

## Architecture

### Day00 - Shared Utilities Library
A class library containing reusable utilities for solving puzzles. Each daily solution project references Day00.

Key utilities:
- **ReadInputs.cs**: Core span-based input parsing from files or stdin. Input file is passed as a command-line argument.
- **ReadInputExtensions.cs**: Higher-level input methods:
  - `Read<T>(Func<IEnumerable<int>, T>)` - Parse space-delimited integers per line
  - `ReadRecords<T>(Func<string[], T>)` - Parse multi-line records separated by blank lines
  - `ReadParts<TContext, TResult>(arrange, execute)` - Parse two-section inputs (common AoC pattern)
  - `ReadIntegers()` - One integer per line
  - `ReadAsRowsOfIntegers()` - Digit grids
  - `ReadSplit()` - Split lines by delimiter
- **Grid.cs / Node.cs**: 2D grid with neighbor traversal (`Up`, `Down`, `Left`, `Right`, diagonals), BFS distance calculations, region detection via `GetRegions()`, and `SequenceEqual` for pattern matching.
- **RenderExtensions.cs**: Fluent `.ToConsole()` extension for outputting results.
- **StringParsingExtensions.cs**: `ParseInt`, `ParseIntegers`, `ParseLongs`, `ParseParts` for extracting numbers/parts from strings.
- **EnumerableExtensions.cs**: `SplitToInts`, `Second()`, `Third()`, `WithoutElementAt()`, `Product()`, `SumOf()`, `ProductOf()`, `MaxOf()`.
- **CombinatoricsEnumerableExtensions.cs**: `Combinations(k)` and `Permutations()` generators.

### Day## - Daily Solution Projects
Each day has its own console application project with:
- `Program.cs` - Solution code (top-level statements)
- `sample.txt` - Example input from the puzzle
- `problem.txt` - Actual puzzle input
- `Properties/Usings.cs` - Global usings including `Day00` namespace and static `ReadInputs`

### Input Handling Pattern
Solutions use the static `Read()` methods from Day00. Input source is determined by command-line args:
- If a file path argument is provided, reads from that file
- Otherwise reads from stdin (type input, then blank line to end)

Common patterns:
```csharp
// Two-section input (rules + data)
ReadParts(
    rules => rules.Select(ParseRule).ToList(),
    (data, rules) => Solve(data, rules))

// Multi-line records
ReadRecords(lines => new Record(lines))
```

### Output Pattern
Solutions typically use `.ToConsole()` for formatted output:
```csharp
result.ToConsole(x => $"Part 1: {x.part1}\nPart 2: {x.part2}");
```

## Creating a New Day

1. Create folder `DayXX` with `DayXX.csproj` referencing `Day00`
2. Add `sample.txt` and `problem.txt` with `CopyToOutputDirectory: Always`
3. Add `Properties/Usings.cs` with global usings for Day00
4. Write solution in `Program.cs` using top-level statements
