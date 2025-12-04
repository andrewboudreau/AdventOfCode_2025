namespace Day00;

public static class GridRenderExtensions
{
    public static void RenderDistances<T>(this Grid<T> grid)
        => grid.Render((n, draw) => draw((n.Distance % 10).ToString()), null);

    public static void Render<T>(this Grid<T> grid, Action<Node<T>, Action<string>> drawCell, Action<string>? draw = default)
    {
        draw ??= Console.Write;
        foreach (var row in grid.Rows())
        {
            foreach (var node in row)
            {
                drawCell(node, draw);
            }

            draw(Environment.NewLine);
        }
    }

    public static void Render<T>(this Grid<T> grid, Action<Node<T>, Action<string>>? drawCell = default, Action<string>? draw = default, int? minX = 0, int? minY = 0, int? maxX = 1000, int? maxY = 1000)
    {
        draw ??= Console.Write;
        drawCell ??= (node, render) => render(node.Value?.ToString() ?? "C");
        foreach (var row in grid.Rows())
        {
            var any = false;
            foreach (var node in row)
            {
                if (minX <= node.X && node.X <= maxX && minY <= node.Y && node.Y <= maxY)
                {
                    drawCell(node, draw);
                    any = true;
                }
            }

            if (any)
            {
                draw(Environment.NewLine);
            }
        }
    }

    public static void Render<T>(this Grid<T> grid, Dictionary<(int X, int Y), string> display, Action<string?>? draw = default)
    {
        draw ??= Console.Write;
        foreach (var row in grid.Rows())
        {
            foreach (var node in row)
            {
                if (display.TryGetValue((node.X, node.Y), out var sprite))
                {
                    draw(sprite);
                }
                else
                {
                    draw(node.Value?.ToString());
                }
            }

            draw(Environment.NewLine);
        }
    }

    public static void Render<T>(this Grid<T> grid, int x = 25, int y = 2, Action<IEnumerable<Node<T>>>? draw = default, Action<int, int>? setPosition = default)
    {
        draw ??= Console.WriteLine;
        setPosition ??= Console.SetCursorPosition;
        foreach (var row in grid.Rows())
        {
            setPosition(x, y++);
            draw(row);
        }
    }

    public static void Render<T>(this Grid<T> grid, Action<string>? draw)
    {
        draw ??= Console.WriteLine;
        foreach (var row in grid.Rows())
        {
            draw(string.Join("", row.Select(x => x.Value)));
            //draw(string.Join("", row.Select(x => $"({x.X},{x.Y})[{x.Value}]")));
        }
    }

    public static void Render<T>(this Grid<T> grid, (int X, int Y, int Size) window)
    {
        bool needsLine = false;
        grid.Each(node =>
        {
            if (node.X > window.X - window.Size && node.X < window.X + window.Size)
            {
                if (node.Y > window.Y - window.Size && node.Y < window.Y + window.Size)
                {
                    needsLine = true;
                    Console.Write(node.Value);
                }
            }

            if (node.X == grid.Width - 1 && needsLine == true)
            {
                Console.WriteLine();
                needsLine = false;
            }
        });
    }
}