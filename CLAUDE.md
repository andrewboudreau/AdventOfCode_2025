# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an Advent of Code 2025 solutions repository implemented in C#. Advent of Code is an annual programming competition with daily puzzles released December 1-25.

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
- **ReadInputs.cs**: Input parsing with multiple overloads for reading from files or stdin. Supports string, span-based, and integer parsing. Input file is passed as a command-line argument.
- **Grid.cs / Node.cs**: 2D grid data structure with neighbor traversal, BFS distance calculations, region detection, and sequence matching. Nodes track position, value, distance, and visited state.
- **RenderExtensions.cs**: Fluent `.ToConsole()` extension for outputting results.
- **StringParsingExtensions.cs**: Helpers like `ParseInt`, `ParseIntegers`, `ParseLongs` for extracting numbers from strings.
- **EnumerableExtensions.cs**: Collection helpers including bit manipulation, `SplitToInts`, `Second()`, `Third()`, `WithoutElementAt()`.

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

### Output Pattern
Solutions typically use `.ToConsole()` for formatted output:
```csharp
result.ToConsole(x => $"Solution to Part 1: {x}");
```

## Creating a New Day

1. Create folder `DayXX` with `DayXX.csproj` referencing `Day00`
2. Add `sample.txt` and `problem.txt` with `CopyToOutputDirectory: Always`
3. Add `Properties/Usings.cs` with global usings for Day00
4. Write solution in `Program.cs` using top-level statements
