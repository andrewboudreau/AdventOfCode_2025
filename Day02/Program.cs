// https://adventofcode.com/2025/day/2

var line = ReadLines().First();
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
            invalidNumbers[0].Add(current);
        }

        if (IsPart2Sequence(current.ToString()))
        {
            invalidNumbers[1].Add(current);
        }
    } while (current++ < end);
}

Console.WriteLine($"Process took {iterations:#,###} iterations.");

invalidNumbers[0].Sum().ToConsole("Part1 - Invalid Ids sum to:");
invalidNumbers[1].Sum().ToConsole("Part2 - Invalid Ids sum to:");

static bool IsPart1Sequence(string value)
{
    var mid = value.Length / 2;
    var left = value[..mid];
    var right = value[mid..];
    return left == right;
}

bool IsPart2Sequence(string value)
{
    for (var size = 1; size <= value.Length / 2; size++)
    {
        if (value.Length % size != 0) continue;

        var pattern = value.AsSpan(0, size);
        var isRepeated = true;

        for (var i = size; i <= value.Length - size; i += size)
        {
            iterations++;
            var chunk = value.AsSpan(i, size);
            if (!chunk.SequenceEqual(pattern))
            {
                isRepeated = false;
                break;
            }
        }

        if (isRepeated)
        {
            return true;
        }
    }

    return false;
}

static bool IsPart2Sequence_Naive(string value)
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
