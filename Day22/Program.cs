using System.Text.RegularExpressions;

class Cuboid
{
    public bool On { get; set; }

    public long FromX { get; set; }
    public long ToX { get; set; }
    public long FromY { get; set; }
    public long ToY { get; set; }
    public long FromZ { get; set; }
    public long ToZ { get; set; }

    public long Size => (ToX - FromX + 1) * (ToY - FromY + 1) * (ToZ - FromZ + 1);
    public bool IsValid => FromX <= ToX && FromY <= ToY && FromZ <= ToZ;

    public List<Cuboid> Except(Cuboid other)
    {
        return new List<Cuboid>
        {
            new Cuboid { FromX = FromX, ToX = ToX, FromY = FromY, ToY = ToY, FromZ = FromZ, ToZ = Math.Min(ToZ, other.FromZ - 1) },
            new Cuboid { FromX = FromX, ToX = ToX, FromY = FromY, ToY = ToY, FromZ = Math.Max(FromZ, other.ToZ + 1), ToZ = ToZ },

            new Cuboid { FromX = FromX, ToX = ToX, FromY = FromY, ToY = Math.Min(ToY, other.FromY - 1), FromZ = Math.Max(FromZ, other.FromZ), ToZ = Math.Min(ToZ, other.ToZ) },
            new Cuboid { FromX = FromX, ToX = ToX, FromY = Math.Max(FromY, other.ToY + 1), ToY = ToY, FromZ = Math.Max(FromZ, other.FromZ), ToZ = Math.Min(ToZ, other.ToZ) },

            new Cuboid { FromX = FromX, ToX = Math.Min(ToX, other.FromX - 1), FromY = Math.Max(FromY, other.FromY), ToY = Math.Min(ToY, other.ToY), FromZ = Math.Max(FromZ, other.FromZ), ToZ = Math.Min(ToZ, other.ToZ) },
            new Cuboid { FromX = Math.Max(FromX, other.ToX + 1), ToX = ToX,  FromY = Math.Max(FromY, other.FromY), ToY = Math.Min(ToY, other.ToY), FromZ = Math.Max(FromZ, other.FromZ), ToZ = Math.Min(ToZ, other.ToZ) }
        }
        .Where(c => c.IsValid)
        .ToList();
    }

    public static Cuboid Parse(string x)
    {
        var regex = new Regex(@"-?\d+");
        var matches = regex.Matches(x);

        var values = matches.Select(m => long.Parse(m.Value)).ToList();

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
    public static long Process(List<Cuboid> cuboids)
    {
        var on = new List<Cuboid>();

        foreach (var cuboid in cuboids)
        {
            if (cuboid.On)
            {
                var parts = new List<Cuboid> { cuboid };

                foreach (var other in on)
                {
                    parts = parts.SelectMany(p => p.Except(other)).ToList();
                }

                on.AddRange(parts);
            }
            else
            {
                on = on.SelectMany(other => other.Except(cuboid)).ToList();
            }
        }

        return on.Sum(c => c.Size);
    }

    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");
        var cuboids = input.Select(Cuboid.Parse).ToList();

        var initializationCuboids = cuboids
            .Where(c => c.FromX >= -50 && c.ToX <= 50 &&
                c.FromY >= -50 && c.ToY <= 50 &&
                c.FromZ >= -50 && c.ToZ <= 50)
            .ToList();

        var answer1 = Process(initializationCuboids);
        Console.WriteLine($"Answer 1: {answer1}");

        var answer2 = Process(cuboids);
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
