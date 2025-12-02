# Day 2: Gift Shop

[Puzzle Link](https://adventofcode.com/2025/day/2)

## Problem

Find "invalid" product IDs within given ranges. An ID is invalid if it consists of a digit sequence repeated exactly twice consecutively (e.g., `55`, `6464`, `123123`).

### Part 1
Sum all invalid IDs found across all ranges.

### Part 2
Same concept but now an ID is invalid if it consists of **any** repeated sequence (not just doubled). For example, `123123123` (123 repeated 3 times) or `7777` (7 repeated 4 times) are now invalid too.

## Solution

**Part 1** uses string slicing to check for exactly-doubled sequences:

```csharp
static bool IsPart1Sequence(string value)
{
    var mid = value.Length / 2;
    var left = value[..mid];
    var right = value[mid..];
    return left == right;
}
```

**Part 2** tries all possible chunk sizes and checks if all chunks are identical:

```csharp
static bool IsPart2Sequence(string value)
{
    for (var size = 1; size <= value.Length / 2; size++)
    {
        if (value.Chunk(size).Select(c => new string(c)).Distinct().Count() == 1)
        {
            return true;
        }
    }
    return false;
}
```

Note: `Chunk()` returns `char[]` arrays which compare by reference, so converting to `string` is needed for `Distinct()` to work correctly.

## Answers
- Part 1: `28846518423`
- Part 2: `31578210022`

