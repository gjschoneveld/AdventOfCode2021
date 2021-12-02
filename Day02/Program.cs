class State
{
    public int Aim { get; set; }
    public int Distance { get; set; }
    public int Depth { get; set; }

}

class ForwardCommand : Command
{
    public override void Apply(State state)
    {
        state.Distance += Units;
        state.Depth += state.Aim * Units;
    }
}

class DownCommand : Command
{
    public override void Apply(State state)
    {
        state.Aim += Units;
    }
}

class UpCommand : Command
{
    public override void Apply(State state)
    {
        state.Aim -= Units;
    }
}

abstract class Command
{
    public int Units { get; set; }

    public abstract void Apply(State state);

    public static Command Parse(string x)
    {
        var parts = x.Split(' ');

        var direction = parts[0];
        var units = int.Parse(parts[1]);

        return direction switch
        {
            "forward" => new ForwardCommand { Units = units },
            "down" => new DownCommand { Units = units },
            "up" => new UpCommand { Units = units },
            _ => throw new Exception()
        };
    }
}

class Program
{
    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");
        var commands = input.Select(Command.Parse).ToList();

        var forward = commands.OfType<ForwardCommand>().Sum(c => c.Units);
        var up = commands.OfType<UpCommand>().Sum(c => c.Units);
        var down = commands.OfType<DownCommand>().Sum(c => c.Units);

        var answer1 = forward * (down - up);

        Console.WriteLine($"Answer 1: {answer1}");


        State state = new();

        foreach (var cmd in commands)
        {
            cmd.Apply(state);
        }

        var answer2 = state.Distance * state.Depth;

        Console.WriteLine($"Answer 2: {answer2}");
    }
}
