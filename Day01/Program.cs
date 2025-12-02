// https://adventofcode.com/2025/day/1

var dial = 50;
var zeros = 0;

Read(rot =>
{
    int d = int.Parse(rot[1..]);
    bool startingAtZero = dial == 0;
    bool wrapped = false;

    if (rot[0] == 'L')
    {
        Console.Write($"Left {d}");
        dial -= d;
    }
    else // rot[0] == 'R'
    {
        Console.Write($"Right {d}");
        dial += d;
    }

    while (dial < 0)
    {
        if (!startingAtZero)
        {
            Console.Write(" (wrapped)");
            zeros++;
        }
        startingAtZero = false;
        dial += 100;
        wrapped = true;
    }
    while (dial > 100)
    {
        if (!startingAtZero)
        {
            Console.Write(" (wrapped)");
            zeros++;
        }
        startingAtZero = false;
        dial -= 100; 
    }

    if (dial == 0 || dial == 100)
    {
        Console.Write(" (zero)");
        if (!wrapped)
            zeros++;

        dial = 0;
    }
    Console.WriteLine($", now at {dial} with {zeros} zeros");
});

// part1 answer is 1097
// part2 7542 is too high
// part2 5793 is too low
// part2 6971 is not right 6672
// part2 6738 is not correct

zeros.ToConsole($"The dial pointed at zero {zeros} times.");