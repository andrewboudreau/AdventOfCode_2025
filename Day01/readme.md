# Day 1: Secret Entrance

[Puzzle Link](https://adventofcode.com/2025/day/1)

## Problem

A safe dial with numbers 0-99 arranged in a circle, starting at position 50. Given rotation instructions (L/R + distance), determine how many times the dial points at 0.

- **L** = rotate left (toward lower numbers)
- **R** = rotate right (toward higher numbers)
- Dial wraps: left from 0 goes to 99, right from 99 goes to 0

### Part 1
Count how many times the dial **stops** at 0 after completing a rotation.

### Part 2
Count **every click** that causes the dial to point at 0, including during rotations (not just at the end).

## Solution

Uses the `CircularRange` [class from Day00](../Day00/CircularRange.cs) which calculates zero crossings in O(1) using modular arithmetic:

```csharp
var dial = new CircularRange();

Read(rot =>
{
    int distance = int.Parse(rot[1..]);
    if (rot[0] == 'L')
        dial.MoveBackward(distance);
    else
        dial.MoveForward(distance);
});

dial.StoppedOnZero.ToConsole(x => $"Part 1: {x}");
dial.CrossedZero.ToConsole(x => $"Part 2: {x}");
```

## Naive Solution

`NaiveSolution.cs` contains the original click-by-click simulation approach - simpler to understand but O(distance) per rotation:

```csharp
do
{
    dial--;
    if (dial == 0) crossedZero++;
    else if (dial == -1) dial = 99;
}
while (--move > 0);
```

## Answers
- Part 1: `1097`
- Part 2: `7101`
