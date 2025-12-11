using System.Collections;
using System.Collections.Immutable;

namespace Day00;

public class Grid<T> : IEnumerable<Node<T>>
{
    private readonly List<Node<T>> nodes;
    private readonly int width;
    private readonly int height;

    public Grid(IEnumerable<string> rows, Func<string, IEnumerable<T>> factory)
       : this(rows.Select(factory))
    {
    }

    public Grid(IEnumerable<IEnumerable<T>> map, Action<Node<T>>? onCreate = default)
    {
        nodes = [];
        int x = 0;
        int y = 0;

        foreach (var row in map)
        {
            foreach (var value in row)
            {
                var node = new Node<T>(x++, y, value);
                nodes.Add(node);
                onCreate?.Invoke(node);
            }

            if (width == 0)
            {
                width = x;
            }
            x = 0;
            y++;
            height++;
        }
    }

    public Node<T>? this[int x, int y]
    {
        get
        {
            if (x < 0) return default;
            if (x >= width) return default;
            if (y < 0) return default;
            if (y >= height) return default;

            int offset = y * width + x;
            if (offset < 0 || offset >= nodes.Count) return default;
            return nodes[offset];
        }
    }

    public Node<T>? this[(int X, int Y) position]
    {
        get => this[position.X, position.Y];
    }

    public int Width => width;
    public int Height => height;

    public IEnumerable<Node<T>> Neighbors(Node<T> of, bool withDiagonals = true)
    {
        if (withDiagonals && this[of.X + 1, of.Y - 1] is Node<T> downRight)
        {
            yield return downRight;
        }

        if (this[of.X + 1, of.Y] is Node<T> right)
        {
            yield return right;
        }

        if (this[of.X, of.Y - 1] is Node<T> down)
        {
            yield return down;
        }

        if (withDiagonals && this[of.X - 1, of.Y - 1] is Node<T> downLeft)
        {
            yield return downLeft;
        }

        if (this[of.X - 1, of.Y] is Node<T> left)
        {
            yield return left;
        }

        if (withDiagonals && this[of.X - 1, of.Y + 1] is Node<T> upLeft)
        {
            yield return upLeft;
        }

        if (this[of.X, of.Y + 1] is Node<T> up)
        {
            yield return up;
        }

        if (withDiagonals && this[of.X + 1, of.Y + 1] is Node<T> upRight)
        {
            yield return upRight;
        }
    }

    public virtual IEnumerable<Node<T>> Nodes() => nodes;

    public void FillDistances(Node<T> from)
    {
        this.ResetVisited();
        this.ResetDistances();

        from.SetDistance(0);
        var nodes = new Queue<Node<T>>();
        nodes.Enqueue(from);

        while (nodes.TryDequeue(out var current))
        {
            foreach (var node in Nodes().Where(x => x.Neighbors.Contains(current)).Except(nodes))
            {
                if (current.Distance + 1 < node.Distance)
                {
                    nodes.Enqueue(node);
                    node.SetDistance(current.Distance + 1);
                }
            }
        }
    }

    public Grid<T> WhileTrue(Func<Grid<T>, bool> operation)
    {
        while (operation(this)) ;
        return this;
    }

    public Grid<T> Until(Func<Grid<T>, bool> operation)
    {
        while (!operation(this)) ;
        return this;
    }

    public Grid<T> Each(Action<Node<T>> action)
    {
        foreach (var node in Nodes())
        {
            action(node);
        }

        return this;
    }

    public int ManhattanDistance(Node<T> from, Node<T> to)
        => Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);

    public virtual IEnumerable<IEnumerable<Node<T>>> Rows()
    {
        for (var row = 0; row < nodes.Count / width; row++)
        {
            yield return nodes.Skip(row * width).Take(width);
        }
    }

    /// <summary>
    /// Returns true if the sequence of the nodes matches for the given function and direction.
    /// </summary>
    /// <param name="sequenceToMatch">The sequence to match.</param>
    /// <param name="startAt">The starting node, this node is compared to the first node in <paramref name="sequenceToMatch"/>.</param>
    /// <param name="nextNode">The function to get the next node, given the current node.</param>
    /// <returns>True if the sequence was matched, false otherwise.</returns>
    public bool SequenceEqual(ReadOnlySpan<T> sequenceToMatch, Node<T> startAt, Func<Node<T>, Node<T>?> nextNode, Func<T, T, bool>? areEqual = default)
    {
        areEqual ??= (a, b) => a is not null && a.Equals(b);
        Node<T>? current = startAt;

        foreach (var test in sequenceToMatch)
        {

            if (current is null || current.Value is null || test is null)
            {
                return false;
            }

            //Console.WriteLine($"Testing {test} at {current}");
            if (!areEqual(current.Value, test))
            {
                return false;
            }

            current = nextNode(current);
        }

        return true;
    }

    public IEnumerator<Node<T>> GetEnumerator()
        => Nodes().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public Node<T>? Up(Node<T> node) => this[node.X, node.Y - 1];
    public Node<T>? Down(Node<T> node) => this[node.X, node.Y + 1];
    public Node<T>? Left(Node<T> node) => this[node.X - 1, node.Y];
    public Node<T>? Right(Node<T> node) => this[node.X + 1, node.Y];
    public Node<T>? UpRight(Node<T> node) => this[node.X + 1, node.Y - 1];
    public Node<T>? UpLeft(Node<T> node) => this[node.X - 1, node.Y - 1];
    public Node<T>? DownRight(Node<T> node) => this[node.X + 1, node.Y + 1];
    public Node<T>? DownLeft(Node<T> node) => this[node.X - 1, node.Y + 1];

    /// <summary>
    /// Returns all nodes in a column (top to bottom).
    /// </summary>
    public IEnumerable<Node<T>> Column(int x)
    {
        for (int y = 0; y < height; y++)
        {
            if (this[x, y] is Node<T> node)
                yield return node;
        }
    }

    /// <summary>
    /// Returns all nodes in a row (left to right).
    /// </summary>
    public IEnumerable<Node<T>> Row(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (this[x, y] is Node<T> node)
                yield return node;
        }
    }

    /// <summary>
    /// Returns a vertical slice (column segment) using Range syntax.
    /// Usage: grid.Slice(x: 5, y: 0..3) returns nodes at (5,0), (5,1), (5,2)
    /// </summary>
    public IEnumerable<Node<T>> Slice(int x, Range y)
    {
        var (startY, lengthY) = y.GetOffsetAndLength(height);
        for (int row = startY; row < startY + lengthY; row++)
        {
            if (this[x, row] is Node<T> node)
                yield return node;
        }
    }

    /// <summary>
    /// Returns a horizontal slice (row segment) using Range syntax.
    /// Usage: grid.Slice(x: 2..5, y: 3) returns nodes at (2,3), (3,3), (4,3)
    /// </summary>
    public IEnumerable<Node<T>> Slice(Range x, int y)
    {
        var (startX, lengthX) = x.GetOffsetAndLength(width);
        for (int col = startX; col < startX + lengthX; col++)
        {
            if (this[col, y] is Node<T> node)
                yield return node;
        }
    }

    /// <summary>
    /// Returns a 2D rectangular region using Range syntax.
    /// Usage: grid.Region(x: 2..5, y: 1..4) returns a 3x3 region
    /// Iterates row by row (top to bottom, left to right within each row).
    /// </summary>
    public IEnumerable<Node<T>> Region(Range x, Range y)
    {
        var (startX, lengthX) = x.GetOffsetAndLength(width);
        var (startY, lengthY) = y.GetOffsetAndLength(height);

        for (int row = startY; row < startY + lengthY; row++)
        {
            for (int col = startX; col < startX + lengthX; col++)
            {
                if (this[col, row] is Node<T> node)
                    yield return node;
            }
        }
    }

    /// <summary>
    /// Returns a 2D rectangular region as rows (for row-by-row processing).
    /// Usage: grid.RegionRows(x: 2..5, y: 1..4) returns IEnumerable of rows
    /// </summary>
    public IEnumerable<IEnumerable<Node<T>>> RegionRows(Range x, Range y)
    {
        var (startX, lengthX) = x.GetOffsetAndLength(width);
        var (startY, lengthY) = y.GetOffsetAndLength(height);

        for (int row = startY; row < startY + lengthY; row++)
        {
            yield return GetRegionRow(startX, lengthX, row);
        }
    }

    private IEnumerable<Node<T>> GetRegionRow(int startX, int lengthX, int row)
    {
        for (int col = startX; col < startX + lengthX; col++)
        {
            if (this[col, row] is Node<T> node)
                yield return node;
        }
    }

    /// <summary>
    /// Returns a 2D rectangular region as columns (for column-by-column processing).
    /// Usage: grid.RegionColumns(x: 2..5, y: 1..4) returns IEnumerable of columns
    /// </summary>
    public IEnumerable<IEnumerable<Node<T>>> RegionColumns(Range x, Range y)
    {
        var (startX, lengthX) = x.GetOffsetAndLength(width);
        var (startY, lengthY) = y.GetOffsetAndLength(height);

        for (int col = startX; col < startX + lengthX; col++)
        {
            yield return GetRegionColumn(col, startY, lengthY);
        }
    }

    private IEnumerable<Node<T>> GetRegionColumn(int col, int startY, int lengthY)
    {
        for (int row = startY; row < startY + lengthY; row++)
        {
            if (this[col, row] is Node<T> node)
                yield return node;
        }
    }
}

public static class GridExtensions
{
    public static int Count<T>(this Grid<T> grid, Func<Grid<T>, Node<T>, bool> predicate)
    {
        int count = 0;
        foreach (var node in grid)
        {
            if (predicate(grid, node))
            {
                count++;
            }
        }
        return count;
    }

    public static IEnumerable<Node<T>> UpFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var i = node.Y - 1; i >= 0; i--)
        {
            yield return grid[node.X, i]!;
        }
    }

    public static IEnumerable<Node<T>> DownFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var i = node.Y + 1; i < grid.Height; i++)
        {
            yield return grid[node.X, i]!;
        }
    }

    public static IEnumerable<Node<T>> LeftFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var i = node.X - 1; i >= 0; i--)
        {
            yield return grid[i, node.Y]!;
        }
    }

    public static IEnumerable<Node<T>> RightFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (var i = node.X + 1; i < grid.Width; i++)
        {
            yield return grid[i, node.Y]!;
        }
    }

    public static IEnumerable<Node<T>> UpLeftFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (int x = node.X - 1, y = node.Y - 1; x >= 0 && y >= 0; x--, y--)
        {
            yield return grid[x, y]!;
        }
    }

    public static IEnumerable<Node<T>> UpRightFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (int x = node.X + 1, y = node.Y - 1; x < grid.Width && y >= 0; x++, y--)
        {
            yield return grid[x, y]!;
        }
    }

    public static IEnumerable<Node<T>> DownLeftFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (int x = node.X - 1, y = node.Y + 1; x >= 0 && y < grid.Height; x--, y++)
        {
            yield return grid[x, y]!;
        }
    }

    public static IEnumerable<Node<T>> DownRightFrom<T>(this Grid<T> grid, Node<T> node)
    {
        for (int x = node.X + 1, y = node.Y + 1; x < grid.Width && y < grid.Height; x++, y++)
        {
            yield return grid[x, y]!;
        }
    }

    public static void ResetVisited<T>(this Grid<T> grid)
        => grid.Each(node => node.ResetVisited());

    public static void ResetDistances<T>(this Grid<T> grid)
        => grid.Each(node => node.ResetDistance());

    public static void ClearNeighbors<T>(this Grid<T> grid)
        => grid.Each(node => node.ClearNeighbors());

    public static IEnumerable<List<Node<T>>> GetRegions<T>(this Grid<T> grid)
    {
        foreach (var node in grid.Nodes())
        {
            if (node.IsVisited)
            {
                continue;
            }

            node.Visit();
            List<Node<T>> region = [node];
            var queue = new Queue<Node<T>>(region);
            while (queue.TryDequeue(out var current))
            {
                foreach (var neighbor in grid.Neighbors(current, withDiagonals: false))
                {
                    if (neighbor.IsVisited)
                    {
                        continue;
                    }

                    if (neighbor.Value is null)
                    {
                        throw new InvalidOperationException("Value cannot be null");
                    }

                    if (neighbor.Value.Equals(current.Value))
                    {
                        neighbor.Visit();
                        region.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            yield return region;
        }
    }

    public static void SetNeighbors<T>(this Grid<T> grid, Func<Node<T>, bool>? include = default, bool withDiagonals = false)
    {
        grid.Each(n => n.ClearNeighbors());

        include ??= _ => true;
        grid.Each(n => n.AddNeighbors([.. grid.Neighbors(n, withDiagonals).Where(include)]));
    }
}
