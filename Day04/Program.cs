// https://adventofcode.com/2025/day/4

var grid = new Grid<char>(ReadLines()!, line => line);
var accessible = 0;

foreach (var node in grid)
{
    if (node is { Value: '.' })
        continue;

    if (grid.Neighbors(node).Count(n => n.Value == '@') < 4)
    {
        accessible++;
    }
}

accessible.ToConsole(result => $"There were {result} bundles with less than 4 neighbors.");


// Part Two: Keep removing accessible rolls until none are left
var totalRemoved = 0;
bool removedAny;

do
{
    removedAny = false;
    var toRemove = new List<Node<char>>();

    foreach (var node in grid)
    {
        if (node is { Value: '.' })
            continue;

        if (grid.Neighbors(node).Count(n => n.Value == '@') < 4)
        {
            toRemove.Add(node);
        }
    }

    foreach (var node in toRemove)
    {
        node.SetValue('.');
        totalRemoved++;
        removedAny = true;
    }

} while (removedAny);

totalRemoved.ToConsole(result => $"Total rolls removed: {result}");

