// https://adventofcode.com/2025/day/10
// --- Day 10: Factory ---

using System.Collections;

var machines = ReadLines().Select(Machine.Parse).ToList();

foreach (var m in machines)
{
    Console.WriteLine(m);
    Console.WriteLine(m.ToBitString());
}

class Machine
{
    public bool[] Target { get; }
    public int[][] Buttons { get; }

    // BitArray representations
    public BitArray TargetBits { get; }
    public BitArray[] ButtonBits { get; }

    public int LightCount => Target.Length;
    public int ButtonCount => Buttons.Length;

    public Machine(bool[] target, int[][] buttons)
    {
        Target = target;
        Buttons = buttons;

        // Create BitArray for target
        TargetBits = new BitArray(target);

        // Create BitArray for each button (which lights it toggles)
        ButtonBits = buttons.Select(indices =>
        {
            var bits = new BitArray(target.Length);
            foreach (var idx in indices)
                bits[idx] = true;
            return bits;
        }).ToArray();
    }

    public static Machine Parse(string line)
    {
        // Format: [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}

        // Extract target state from [...]
        var bracketStart = line.IndexOf('[');
        var bracketEnd = line.IndexOf(']');
        var targetStr = line[(bracketStart + 1)..bracketEnd];
        var target = targetStr.Select(c => c == '#').ToArray();

        // Extract buttons from (...) groups, ignore {...} joltage
        var buttons = new List<int[]>();
        var remaining = line[(bracketEnd + 1)..];

        var i = 0;
        while (i < remaining.Length)
        {
            if (remaining[i] == '(')
            {
                var end = remaining.IndexOf(')', i);
                var content = remaining[(i + 1)..end];
                var indices = content.Split(',').Select(int.Parse).ToArray();
                buttons.Add(indices);
                i = end + 1;
            }
            else if (remaining[i] == '{')
            {
                // Stop at joltage section
                break;
            }
            else
            {
                i++;
            }
        }

        return new Machine(target, buttons.ToArray());
    }

    public override string ToString()
    {
        var targetStr = string.Join("", Target.Select(b => b ? '#' : '.'));
        var buttonsStr = string.Join(" ", Buttons.Select(b => $"({string.Join(",", b)})"));
        return $"[{targetStr}] {buttonsStr}";
    }

    public string ToBitString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Target: {BitStr(TargetBits)}");
        for (int i = 0; i < ButtonBits.Length; i++)
            sb.AppendLine($"  B{i}:   {BitStr(ButtonBits[i])}");
        return sb.ToString();
    }

    private static string BitStr(BitArray bits)
    {
        var chars = new char[bits.Length];
        for (int i = 0; i < bits.Length; i++)
            chars[i] = bits[i] ? '1' : '0';
        return new string(chars);
    }
}
