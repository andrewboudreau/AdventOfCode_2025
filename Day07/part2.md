# Part 2: Counting Timelines (Many-Worlds Path Counting)

## The Problem

A single particle enters at `S` and moves downward. At each splitter `^`, the particle takes **either** the left path **or** the right path (not both simultaneously). We need to count how many distinct complete journeys (timelines) are possible.

## Visualizing with the Sample

```
.......S.......   <- Particle starts here
.......|.......
.......^.......   <- First choice: go LEFT or RIGHT?
.......|.......
......^.^......   <- More choices...
```

At the first splitter, the particle can go left OR right. Each of those paths may hit more splitters, creating more branches. We're counting the total number of unique paths from start to exit.

## The Naive Approach (Why It's Slow)

You might think: "Just simulate every path!"

```
Timeline 1: S -> down -> splitter -> LEFT -> down -> splitter -> LEFT -> ...exit
Timeline 2: S -> down -> splitter -> LEFT -> down -> splitter -> RIGHT -> ...exit
Timeline 3: S -> down -> splitter -> RIGHT -> down -> splitter -> LEFT -> ...exit
...
```

But with 21 splitters, you could have up to 2^21 = 2 million+ paths. The actual answer for the problem input is **422 trillion** timelines - we can't enumerate them all!

## The Key Insight: Subproblem Overlap

Look at this pattern:

```
.......S.......
.......|.......
......|^|......   <- Splitter A
......|.|......
.....|^|^|.....   <- Splitters B and C
```

If the particle goes LEFT at A, it eventually reaches B.
If the particle goes RIGHT at A, it eventually reaches C.

But here's the key: **Both B and C might lead to the same splitter D later!**

```
.....|^|^|.....   <- B and C
.....|.|.|.....
....|^|^|^|....   <- Splitter D is reachable from BOTH B and C
```

If we already calculated "how many timelines exist starting from D", we don't need to recalculate it when we arrive at D from a different path.

## The Recursive Formula

For any position (x, y):

```
CountTimelines(x, y) =
    if out of bounds -> 1  (particle exited = 1 complete timeline)
    if splitter '^'  -> CountTimelines(LEFT) + CountTimelines(RIGHT)
    if empty '.'     -> CountTimelines(below)
```

**Why add for splitters?** Because the particle goes left OR right. If there are 5 ways to exit going left, and 3 ways going right, there are 5 + 3 = 8 total ways.

## Walking Through the Code

```csharp
var memo = new Dictionary<(int x, int y), long>();

long CountTimelines(int x, int y)
{
    // Base case: particle exits the grid = 1 complete timeline
    if (grid[x, y] is not Node<char> node)
        return 1;

    // Have we already computed this position?
    if (memo.TryGetValue((x, y), out var cached))
        return cached;

    long result;
    if (node.Value == '^')
    {
        // Splitter: total timelines = left timelines + right timelines
        result = CountTimelines(x - 1, y) + CountTimelines(x + 1, y);
    }
    else
    {
        // Empty space: just pass through to below
        result = CountTimelines(x, y + 1);
    }

    // Store result so we never recompute this position
    memo[(x, y)] = result;
    return result;
}
```

## Memoization Explained

Think of `memo` as a notebook where you write down answers:

> "At position (7, 14), there are 40 ways to exit the grid."

**Without memo:** Every time you reach (7, 14) - whether from the left, right, or above - you'd recalculate all 40 paths.

**With memo:** First time at (7, 14), you calculate 40 and write it down. Every subsequent visit, you just look up "40" instantly.

## Concrete Example

Let's trace through a tiny grid:

```
..S..
.....
..^..
.....
.^.^.
.....
```

Call `CountTimelines(2, 1)` (one below S):

1. Position (2,1) is `.` -> go to (2,2)
2. Position (2,2) is `^` -> LEFT + RIGHT = `CountTimelines(1,2)` + `CountTimelines(3,2)`
3. Position (1,2) is `.` -> go to (1,3)
4. Position (1,3) is `.` -> go to (1,4)
5. Position (1,4) is `^` -> LEFT + RIGHT = `CountTimelines(0,4)` + `CountTimelines(2,4)`
6. Position (0,4) is `.` -> go to (0,5)
7. Position (0,5) is `.` -> go to (0,6) -> **OUT OF BOUNDS = 1**
8. Position (2,4) is `.` -> go to (2,5)
9. Position (2,5) is `.` -> go to (2,6) -> **OUT OF BOUNDS = 1**
10. So position (1,4) = 1 + 1 = **2**
11. Back to step 3: position (1,2) eventually = **2** (stored in memo)

*...similar for the right branch...*

12. Position (3,2) is `.` -> eventually reaches (3,4) which is `^`
13. Position (3,4) = `CountTimelines(2,4)` + `CountTimelines(4,4)`
14. **Wait!** We already know `CountTimelines(2,4)` from step 8-9 = **1** (memo hit!)
15. Position (4,4) is `.` -> eventually = **1**
16. So position (3,4) = 1 + 1 = **2**
17. Position (3,2) = **2**

**Final:** Position (2,2) = 2 + 2 = **4 timelines**

## Why Memoization Makes This Fast

| Without Memo | With Memo |
|--------------|-----------|
| Visits (2,4) twice, recalculates both times | Visits (2,4) twice, calculates once |
| O(2^n) where n = splitter count | O(grid cells) = O(width x height) |
| Would take years for 422 trillion paths | Runs in milliseconds |

## The Mental Model

Think of the grid as a tree of choices, but **upside down and merged**:

```
        S
        |
        ^         <- 2 branches
       / \
      ^   ^       <- 4 branches... but wait
     /|   |\
    ^ ^ ^ ^ ^     <- some of these are THE SAME position!
```

Memoization recognizes when branches **reconverge** to the same position and reuses the already-computed answer.

---

## Key Takeaways

1. **Recursion** naturally expresses "count all paths"
2. **Adding at splitters** (left paths + right paths) gives total possibilities
3. **Memoization** avoids redundant work when paths reconverge to the same cell
