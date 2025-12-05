# Day 5: Cafeteria

[Puzzle Link](https://adventofcode.com/2025/day/5)

## Problem

Determine which ingredient IDs are fresh based on a database of valid ID ranges.

**Input format:** Two sections separated by a blank line:
1. Fresh ingredient ID ranges (inclusive, can overlap)
2. Available ingredient IDs to check

## Solution

### Part 1
Check each available ingredient ID against the ranges - count how many fall within any range.

### Part 2
Count total unique fresh IDs across all ranges. Since ranges can overlap, we merge them first.

**Range Merging Algorithm:**
1. Sort ranges by start value (process left-to-right)
2. Walk through ranges, either adding new ranges or extending the last one if they overlap

| Step | Range | Accumulated     | Condition      | Action | Result           |
|------|-------|-----------------|----------------|--------|------------------|
| 1    | 3-5   | []              | empty          | add    | [3-5]            |
| 2    | 10-14 | [3-5]           | 5 < 9? yes     | add    | [3-5, 10-14]     |
| 3    | 12-18 | [3-5, 10-14]    | 14 < 11? no    | merge  | [3-5, 10-18]     |
| 4    | 16-20 | [3-5, 10-18]    | 18 < 15? no    | merge  | [3-5, 10-20]     |

Final: `[3-5, 10-20]` → sizes 3 + 11 = **14**

## Key Techniques
- `ReadParts` helper to parse two-section input with shared context
- `Aggregate` for building merged range list in single pass
- `acc[^1]` index-from-end syntax to access last element
