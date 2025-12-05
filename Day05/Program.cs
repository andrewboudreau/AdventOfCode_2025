// https://adventofcode.com/2025/day/5
// --- Day 5: Cafeteria ---

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

        // Merge overlapping ranges
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

