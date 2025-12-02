//// https://adventofcode.com/2025/day/1

var dial = 50;
int[] zeros = [0, 0];

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
                zeros[1]++;
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
                zeros[1]++;
                dial = 0;
            }
        }
        while (--move > 0);
    }

    if (dial == 0) zeros[0]++;
});

zeros[0].ToConsole($"The dial stopped at zero {zeros[0]} times.");
zeros[1].ToConsole($"The dial crossed zero {zeros[1]} times.");