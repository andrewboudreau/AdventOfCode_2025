// https://adventofcode.com/2025/day/3

using Day00;

int part1 = 0;
long part2 = 0;

foreach (var bank in ReadAsRowsOfIntegers())
{
    var maxValue = FindMaxTwoDigitValue(bank);
    part1 += maxValue;
    Console.WriteLine($"Max 2-Digit value in bank [{string.Join(", ", bank)}] is {maxValue}");

    var maxValuePart2 = FindMaxTweleveDigitValue(bank);
    part2 += maxValuePart2;
    Console.WriteLine($"Max 12-digit value in bank [{string.Join(", ", bank)}] is {maxValuePart2}");
}

part1.ToConsole(sum => $"Total sum of max 2-digit values: {sum}");
part2.ToConsole(sum => $"Total sum of max 12-digit values: {sum}");

static int FindMaxTwoDigitValue(IEnumerable<int> values)
{
    int tensPlace = 0;
    int max = 0;

    foreach (var v in values)
    {
        if (tensPlace == 0)
        {
            tensPlace = v;
            continue;
        }

        var current = (tensPlace * 10) + v;
        if (current > max)
        {
            max = current;
        }
        if (v > tensPlace)
        {
            tensPlace = v;
        }
    }

    return max;
}

static long FindMaxTweleveDigitValue(IEnumerable<int> values)
{
    //start from the right of the value 12 places in, and load that up
    var digits = values.ToArray();
    var max = digits[^12..];

    for (var i = digits.Length - 13; i >= 0; i--)
    {
        var candidate = digits[i..(i + 12)];
        if (IsGreaterThan(candidate, max))
        {
            max = candidate;
        }
    }

    return max.Aggregate(0L, (acc, digit) => acc * 10 + digit);
}

static bool IsGreaterThan(int[] candidate, int[] max)
{
    for (var i = 0; i < candidate.Length; i++)
    {
        if (candidate[i] > max[i])
        {
            return true;
        }
        else if (candidate[i] < max[i])
        {
            return false;
        }
    }

    return false;
}

//part1: 17207