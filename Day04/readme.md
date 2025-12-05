# Day 4: Printing Department

[Puzzle Link](https://adventofcode.com/2025/day/4)

## Problem

Help forklifts access rolls of paper (`@`) arranged on a grid. A roll is accessible if it has fewer than 4 neighboring rolls (using all 8 directions including diagonals).

### Part 1
Count how many rolls of paper can be accessed by a forklift (fewer than 4 `@` neighbors).

### Part 2
Once a roll is removed, new rolls may become accessible. Keep removing accessible rolls until no more can be accessed. Count the total number of rolls removed.

## Solution

**Part 1** iterates through the grid and counts rolls with fewer than 4 `@` neighbors:

```csharp
foreach (var node in grid)
{
    if (node is { Value: '.' })
        continue;

    if (grid.Neighbors(node).Count(n => n.Value == '@') < 4)
    {
        accessible++;
    }
}
```

**Part 2** uses `grid.WhileTrue` to repeatedly find and remove accessible rolls until none remain:

```csharp
grid.WhileTrue(g =>
{
    var toRemove = new List<Node<char>>();
    foreach (var node in grid)
    {
        if (node.Value == '@' && g.Neighbors(node).Count(n => n.Value == '@') < 4)
        {
            toRemove.Add(node);
        }
    }
    
    totalRemoved += toRemove.Count;
    toRemove.ForEach(node => node.SetValue('.'));

    return toRemove.Count > 0;
});
```

## Visualization

The final grid state after all removals, rendered as a bitmap (white = remaining rolls, black = empty):

![Output](output.bmp)

## Answers
- Part 1: `1367`
- Part 2: `9144`
