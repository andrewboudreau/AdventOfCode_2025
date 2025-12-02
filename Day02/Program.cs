// https://adventofcode.com/2025/day/2

var part1 = 0;
var part2 = 1;

var line = ReadLines().First()!;
var ranges = line.Split(',').Select(r => r.Split('-').ToArray()).ToList();

List<long>[] invalidNumbers = [[], []];
long iterations = 0;

foreach (var range in ranges)
{
    var current = long.Parse(range[0]);
    var end = long.Parse(range[1]);

    do
    {
        if (IsPart1Sequence(current.ToString()))
        {
            invalidNumbers[part1].Add(current);
        }

        if (IsPart2Sequence(current.ToString()))
        {
            invalidNumbers[part2].Add(current);
        }
        iterations++;
    } while (current++ < end);
}

Console.WriteLine($"Process took {iterations:#,###} iterations.");

invalidNumbers[part1].Sum().ToConsole("Part1 - Invalid Ids sum to:");
invalidNumbers[part2].Sum().ToConsole("Part2 - Invalid Ids sum to:");

static bool IsPart1Sequence(string value)
{
    var mid = value.Length / 2;
    var left = value[..mid];
    var right = value[mid..];
    return left == right;
}

static bool IsPart2Sequence(string value)
{
    for (var size = value.Length / 2; size > 0; size--)
    {
        if (value.Chunk(size).Select(c => new string(c)).Distinct().Count() == 1)
        {
            return true;
        }
    }

    return false;
}
