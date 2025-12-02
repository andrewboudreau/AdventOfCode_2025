# Day 1: Secret Entrance

[Puzzle Link](https://adventofcode.com/2025/day/1)

## Problem

A safe dial with numbers 0-99 arranged in a circle, starting at position 50. Given rotation instructions (L/R + distance), determine how many times the dial points at 0.

- **L** = rotate left (toward lower numbers)
- **R** = rotate right (toward higher numbers)
- Dial wraps: left from 0 goes to 99, right from 99 goes to 0

## Solution Approach

Click-by-click simulation - process each rotation one click at a time, checking for zeros as we go.

### Part 1
Count how many times the dial **stops** at 0 after completing a rotation.

### Part 2
Count **every click** that causes the dial to point at 0, including during rotations (not just at the end).

## Key Insight

Part 2's edge case: a single rotation like `R1000` from position 50 crosses 0 ten times. Rather than calculating wrap counts mathematically, simulating each click handles all edge cases naturally.

```csharp
// Track both answers simultaneously
int[] zeros = [0, 0];  // [part1, part2]

// Simulate each click
dial--;
if (dial == 0) zeros[1]++;      // Part 2: crossed zero
else if (dial == -1) dial = 99; // Wrap around

// After rotation completes
if (dial == 0) zeros[0]++;      // Part 1: stopped at zero
```

## Answers
- Part 1: `1097`
- Part 2: `6714`
