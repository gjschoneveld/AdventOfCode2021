using System.Text;

class Field
{
    public int Width { get; set; }
    public int Height { get; set; }

    public HashSet<(int x, int y)> East { get; set; } = new();
    public HashSet<(int x, int y)> South { get; set; } = new();

    public (HashSet<(int x, int y)> next, bool moved) Step(HashSet<(int x, int y)> herd, Func<(int x, int y), (int x, int y)> getTarget)
    {
        var next = new HashSet<(int x, int y)>(); 
        var moved = false;

        foreach (var position in herd)
        {
            var target = getTarget(position);

            if (East.Contains(target) || South.Contains(target))
            {
                next.Add(position);
            }
            else
            {
                next.Add(target);
                moved = true;
            }
        }

        return (next, moved);
    }

    public bool Step()
    {
        (East, var movedEast) = Step(East, p => ((p.x + 1) % Width, p.y));
        (South, var movedSouth) = Step(South, p => (p.x, (p.y + 1) % Height));

        return movedEast || movedSouth;
    }

    public override string ToString()
    {
        var result = new StringBuilder();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (East.Contains((x, y)))
                {
                    result.Append('>');
                }
                else if (South.Contains((x, y)))
                {
                    result.Append('v');
                }
                else
                {
                    result.Append('.');
                }
            }

            result.AppendLine();
        }

        return result.ToString();
    }

    public static Field Parse(string[] input)
    {
        var result = new Field
        {
            Width = input[0].Length,
            Height = input.Length
        };

        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == '>')
                {
                    result.East.Add((x, y));
                }
                else if (input[y][x] == 'v')
                {
                    result.South.Add((x, y));
                }
            }
        }

        return result;
    }
}

class Program
{
    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");
        var field = Field.Parse(input);

        int steps = 1;

        while (field.Step())
        {
            steps++;
        }

        var answer = steps;
        Console.WriteLine($"Answer: {answer}");
    }
}
