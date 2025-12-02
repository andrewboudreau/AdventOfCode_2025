// https://adventofcode.com/2025/day/1

var dial = 50;
var zeros = 0;

Read(rot =>
{
    int move = int.Parse(rot[1..]);
    if (rot[0] == 'L')
    {
        do
        {
            dial--;
            if (dial == 0)
            {
                zeros++;
            }
            else if (dial == -1)
            {
                dial = 99;
            }
        }
        while (--move > 0);
    }
    else // rot[0] == 'R'
    {
        do
        {
            dial++;
            if (dial == 100)
            {
                zeros++;
                dial = 0;
            }
        }
        while (--move > 0);
    }
});

zeros.ToConsole($"The dial pointed at zero {zeros} times.");