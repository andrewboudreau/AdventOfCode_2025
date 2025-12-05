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

    public static void RenderToBitmap<T>(this Grid<T> grid, string filePath, Func<T, (byte R, byte G, byte B)> getColor, int scale = 1)
    {
        int scaledWidth = grid.Width * scale;
        int scaledHeight = grid.Height * scale;
        int rowStride = (scaledWidth * 3 + 3) & ~3; // BMP rows must be 4-byte aligned
        int imageSize = rowStride * scaledHeight;
        int fileSize = 54 + imageSize; // 54 byte header + pixel data

        using var stream = File.Create(filePath);
        using var writer = new BinaryWriter(stream);

        // BMP Header (14 bytes)
        writer.Write((byte)'B');
        writer.Write((byte)'M');
        writer.Write(fileSize);
        writer.Write((short)0); // Reserved
        writer.Write((short)0); // Reserved
        writer.Write(54);       // Pixel data offset

        // DIB Header (40 bytes)
        writer.Write(40);       // Header size
        writer.Write(scaledWidth);
        writer.Write(scaledHeight);
        writer.Write((short)1); // Color planes
        writer.Write((short)24); // Bits per pixel
        writer.Write(0);        // No compression
        writer.Write(imageSize);
        writer.Write(2835);     // Horizontal resolution (72 DPI)
        writer.Write(2835);     // Vertical resolution (72 DPI)
        writer.Write(0);        // Colors in palette
        writer.Write(0);        // Important colors

        // Pixel data (bottom-up)
        byte[] row = new byte[rowStride];
        for (int y = grid.Height - 1; y >= 0; y--)
        {
            Array.Clear(row);
            for (int x = 0; x < grid.Width; x++)
            {
                var node = grid[x, y];
                var (r, g, b) = node is not null ? getColor(node.Value) : ((byte)0, (byte)0, (byte)0);
                
                // Write scaled pixels
                for (int sx = 0; sx < scale; sx++)
                {
                    int pixelX = x * scale + sx;
                    row[pixelX * 3] = b;     // BMP uses BGR order
                    row[pixelX * 3 + 1] = g;
                    row[pixelX * 3 + 2] = r;
                }
            }
            
            // Write the same row 'scale' times for vertical scaling
            for (int sy = 0; sy < scale; sy++)
            {
                writer.Write(row);
            }
        }
    }
}