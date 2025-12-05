// https://adventofcode.com/2025/day/5
// --- Day 5: Cafeteria ---

ReadParts(
    ranges => ranges
        .Select(x => x.Split('-'))
        .Select(parts => (Start: long.Parse(parts[0]), End: long.Parse(parts[1])))
        .ToList(),
    (inventoryIds, validRanges) =>
        inventoryIds
            .Select(long.Parse)
            .Count(id => validRanges.Any((r) => id >= r.Start && id <= r.End)))
.ToConsole(x => $"Part 1: {x}"); //782

Read(
    (ReadOnlySpan<char> line) =>
    {
        var split = line.IndexOf('-');
        var value = (long.Parse(line[(split+1)..]) - long.Parse(line[..split])) + 1;
        Console.WriteLine($"The value is " + value);
        return value;
    })
.Sum()
.ToConsole(x => $"Part 2: {x}");

