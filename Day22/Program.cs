using System.Text.RegularExpressions;

class Cuboid
{
    public bool On { get; set; }

    public int FromX { get; set; }
    public int ToX { get; set; }
    public int FromY { get; set; }
    public int ToY { get; set; }
    public int FromZ { get; set; }
    public int ToZ { get; set; }

    public List<(int x, int y, int z)> Points()
    {
        var result = new List<(int x, int y, int z)> ();

        for (int x = FromX; x <= ToX; x++)
        {
            for (int y = FromY; y <= ToY; y++)
            {
                for (int z = FromZ; z <= ToZ; z++)
                {
                    result.Add((x, y, z));
                }
            }
        }

        return result;
    }

    public static Cuboid Parse(string x)
    {
        var regex = new Regex(@"-?\d+");
        var matches = regex.Matches(x);

        var values = matches.Select(m => int.Parse(m.Value)).ToList();

        return new Cuboid
        {
            On = x.StartsWith("on"),
            FromX = values[0],
            ToX = values[1],
            FromY = values[2],
            ToY = values[3],
            FromZ = values[4],
            ToZ = values[5]
        };
    }
}

class Program
{
    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");
        var cuboids = input.Select(Cuboid.Parse).ToList();

        var on = new HashSet<(int x, int y, int z)>();

        var initializationCuboids = cuboids
            .Where(c => c.FromX >= -50 && c.ToX <= 50 &&
                c.FromY >= -50 && c.ToY <= 50 &&
                c.FromZ >= -50 && c.ToZ <= 50)
            .ToList();

        foreach (var cuboid in initializationCuboids)
        {
            if (cuboid.On)
            {
                on.UnionWith(cuboid.Points());
            }
            else
            {
                on.ExceptWith(cuboid.Points());
            }
        }

        var answer1 = on.Count;
        Console.WriteLine($"Answer 1: {answer1}");

    }
}
