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

grid.WhileTrue(g =>
{
    var toRemove = new List<Node<char>>();
    foreach (var node in grid)
    {
        if (node.Value == '@' && g.Neighbors(node).Count(n => n.Value == '@') < 4)
        {
            toRemove.Add(node);
        }
    }
    
    totalRemoved += toRemove.Count;
    toRemove.ForEach(node => node.SetValue('.'));

    return toRemove.Count > 0;
});

grid.Render(Console.WriteLine);

var white = ((byte)255, (byte)255, (byte)255);
var black = (((byte)0, (byte)0, (byte)0));

grid.RenderToBitmap(
    filePath: "output.bmp", 
    getColor: node => node == '@' ? white : black, 
    scale: 4);

totalRemoved.ToConsole(result => $"Total rolls removed: {result}");

