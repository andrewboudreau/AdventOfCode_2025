// https://adventofcode.com/2025/day/1

var dial = new CircularRange();

Read(rot =>
{
    int distance = int.Parse(rot[1..]);
    if (rot[0] == 'L')
        dial.MoveBackward(distance);
    else
        dial.MoveForward(distance);
});

dial.StoppedOnZero.ToConsole(x => $"Part 1: {x}");
dial.CrossedZero.ToConsole(x => $"Part 2: {x}");
