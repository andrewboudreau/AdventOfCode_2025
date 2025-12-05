// https://adventofcode.com/2025/day/5
// --- Day 5: Cafeteria ---
//
// Part 1: Check which ingredient IDs fall within any fresh range
// Part 2: Count total unique fresh IDs by merging overlapping ranges
//
// Range Merging Algorithm:
// 1. Sort ranges by Start value (process left-to-right)
// 2. Use Aggregate to build merged list:
//    - acc[^1] means "last item in list"
//    - r.Start - 1 handles adjacent ranges (e.g., 3-5 and 6-10 should merge)
//    - If list empty OR last range ends before this one starts: add new range
//    - Otherwise they overlap: extend last range's End to cover both
//
// Example walkthrough with ranges: 3-5, 10-14, 12-18, 16-20 (sorted)
// | Step | r     | acc (before)     | Condition      | Action | acc (after)      |
// |------|-------|------------------|----------------|--------|------------------|
// | 1    | 3-5   | []               | empty          | add    | [3-5]            |
// | 2    | 10-14 | [3-5]            | 5 < 9? yes     | add    | [3-5, 10-14]     |
// | 3    | 12-18 | [3-5, 10-14]     | 14 < 11? no    | merge  | [3-5, 10-18]     |
// | 4    | 16-20 | [3-5, 10-18]     | 18 < 15? no    | merge  | [3-5, 10-20]     |
// Result: [3-5, 10-20] -> sizes 3 + 11 = 14

ReadParts(
    ranges => ranges
        .Select(x => x.Split('-'))
        .Select(parts => (Start: long.Parse(parts[0]), End: long.Parse(parts[1])))
        .ToList(),
    (inventoryIds, validRanges) =>
    {
        var part1 = inventoryIds
            .Select(long.Parse)
            .Count(id => validRanges.Any(r => id >= r.Start && id <= r.End));

        // Merge overlapping ranges (see algorithm notes above)
        var merged = validRanges
            .OrderBy(r => r.Start)
            .Aggregate(new List<(long Start, long End)>(), (acc, r) =>
            {
                if (acc.Count == 0 || acc[^1].End < r.Start - 1)
                    acc.Add(r);
                else
                    acc[^1] = (acc[^1].Start, Math.Max(acc[^1].End, r.End));
                return acc;
            });

        var part2 = merged.Sum(r => r.End - r.Start + 1);

        return (part1, part2);
    })
.ToConsole(solution => $"Part 1: {solution.part1}\nPart 2: {solution.part2}");

