enum Direction
{
    Forward,
    Down,
    Up
}
class Command
{
    public Direction Direction { get; set; }
    public int Units { get; set; }

    public static Command Parse(string x)
    {
        var parts = x.Split(' ');

        return new()
        {
            Direction = parts[0] switch
            {
                "forward" => Direction.Forward,
                "down" => Direction.Down,
                "up" => Direction.Up,
                _ => throw new Exception()
            },
            Units = int.Parse(parts[1])
        };
    }
}

class Program
{
    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");
        var commands = input.Select(Command.Parse).ToList();

        var forward = commands.Where(c => c.Direction == Direction.Forward).Sum(c => c.Units);
        var up = commands.Where(c => c.Direction == Direction.Up).Sum(c => c.Units);
        var down = commands.Where(c => c.Direction == Direction.Down).Sum(c => c.Units);

        var answer1 = forward * (down - up);

        Console.WriteLine($"Answer 1: {answer1}");


        int aim = 0;
        int distance = 0;
        int depth = 0;

        foreach (var cmd in commands)
        {
            switch (cmd.Direction)
            {
                case Direction.Forward:
                    distance += cmd.Units;
                    depth += aim * cmd.Units;
                    break;

                case Direction.Down:
                    aim += cmd.Units;
                    break;

                case Direction.Up:
                    aim -= cmd.Units;
                    break;
            }
        }

        var answer2 = distance * depth;

        Console.WriteLine($"Answer 2: {answer2}");
    }
}
