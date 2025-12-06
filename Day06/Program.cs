// https://adventofcode.com/2025/day/6
// --- Day 6: Trash Compactor ---

List<List<int>> columns = [];
List<char> operators = [];

ReadSplit(rows =>
{
    foreach (var row in rows)
    {
        foreach (var (ordinal, item) in row.Index())
        {
            if (char.IsNumber(item[0]))
            {
                // make sure our pivot table is big enough
                if (columns.Count <= ordinal)
                {
                    columns.Add([]);
                }

                // add to pivot table
                columns[ordinal].Add(int.Parse(item));
            }
            else
            {
                // add to list of operators
                operators.Add(item[0]);
            }
        }
    }
});

long total = 0;
foreach (var (ordinal, op) in operators.Index())
{
    if (op == '+')
    {
        long tmp = 0;
        foreach (var operand in columns[ordinal])
        {
            tmp += operand;
        }
        total += tmp;

    }
    else
    {
        long tmp = 1;
        foreach (var operand in columns[ordinal])
        {
            tmp *= operand;
        }
        total += tmp;
    }

}

total.ToConsole(t => $"The total of the pivot is {t}");
// 26310042713
// 26310042713 is too low