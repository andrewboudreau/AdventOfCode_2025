// https://adventofcode.com/2025/day/3


int part1 = 0;
long part1a = 0;
long part2 = 0;

foreach (var bank in ReadAsRowsOfIntegers())
{
    var maxValue = FindMaxTwoDigitValue(bank);
    var maxValuePart1 = FindLargestNDigitNumber([.. bank], 2);
    part1 += maxValue;
    part1a += maxValuePart1;

    //Console.WriteLine($"Max 2-Digit value in bank [{string.Join(", ", bank)}] is {maxValue}");
    //Console.WriteLine($"Max General 2-Digit value in bank [{string.Join(", ", bank)}] is {maxValuePart1}");

    var maxValuePart2 = FindLargestNDigitNumber([.. bank], 12);
    part2 += maxValuePart2;
    Console.WriteLine($"Max 12-digit value in bank [{string.Join(", ", bank)}] is {maxValuePart2}");
}

part1.ToConsole(sum => $"Total sum of max 2-digit values: {sum}");
part1a.ToConsole(sum => $"General Total sum of max 2-digit values: {sum}");

part2.ToConsole(sum => $"Total sum of max 12-digit values: {sum}");

long FindLargestNDigitNumber(int[] ints, int n)
{
    long result = 0;
    int startIndex = 0;

    for (int digitsNeeded = n; digitsNeeded > 0; digitsNeeded--)
    {
        // Find the largest digit in the range where we can still pick enough remaining digits
        int maxDigit = 0;
        int maxIndex = startIndex;

        // We can search from startIndex up to (ints.Length - digitsNeeded)
        for (int i = startIndex; i <= ints.Length - digitsNeeded; i++)
        {
            if (ints[i] > maxDigit)
            {
                maxDigit = ints[i];
                maxIndex = i;
            }
        }

        result = (result * 10) + maxDigit;
        startIndex = maxIndex + 1;
    }

    return result;
}

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


//part1: 17207