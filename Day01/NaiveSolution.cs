namespace Day01;

/// <summary>
/// Original click-by-click simulation solution for Day 1.
/// </summary>
public static class NaiveSolution
{
    /// <summary>
    /// Solves Day 1 using click-by-click simulation.
    /// </summary>
    /// <returns>Tuple of (stoppedOnZero, crossedZero) counts.</returns>
    public static (int StoppedOnZero, int CrossedZero) Solve()
    {
        var dial = 50;
        int[] zeros = [0, 0]; // [stopped, crossed]

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

        return (zeros[0], zeros[1]);
    }
}
