using System.Text;

class Field : IEquatable<Field>
{
    public HashSet<(int x, int y)> Points { get; set; } = new();
    public List<(char type, (int x, int y) position)> Amphipods { get; set; } = new();
    public int Energy { get; set; } = 0;

    public List<(int x, int y)> EmptyNeighbours((int x, int y) position)
    {
        return new List<(int x, int y)>
        {
            (position.x, position.y - 1),
            (position.x - 1, position.y),
            (position.x + 1, position.y),
            (position.x, position.y + 1)
        }
        .Where(p => Points.Contains(p) && !Amphipods.Any(a => a.position == p))
        .ToList();
    }

    public Dictionary<(int x, int y), int> Moves((int x, int y) start)
    {
        var result = new Dictionary<(int x, int y), int>();

        int steps = 0;
        var current = new List<(int x, int y)> { start };

        while (current.Count > 0)
        {
            current = current
                .SelectMany(EmptyNeighbours)
                .Distinct()
                .Where(p => !result.ContainsKey(p))
                .ToList();

            steps++;

            foreach (var position in current)
            {
                result[position] = steps;
            }
        }

        return result;
    }

    public bool IsHallway((int x, int y) position) => Points.Contains(position) && position.y == 1;
    public bool IsRoom((int x, int y) position) => Points.Contains(position) && !IsHallway(position);

    public List<char> RoomContent(int x)
    {
        return Amphipods.Where(a => a.position.x == x).Select(a => a.type).ToList();
    }

    public int TargetRoom(char type)
    {
        return (type - 'A') * 2 + 3;
    }

    public int StepEnergy(char type)
    {
        return Enumerable.Repeat(10, type - 'A').Aggregate(1, (a, b) => a * b);
    }

    public List<Field> Next()
    {
        var result = new List<Field>();

        foreach (var amphipod in Amphipods)
        {
            if (amphipod.position.x == TargetRoom(amphipod.type) && !RoomContent(amphipod.position.x).Any(t => t != amphipod.type))
            {
                // no need to move
                continue;
            }

            var candidateMoves = Moves(amphipod.position);

            var validMoves = candidateMoves.Where(c =>
            {
                var position = c.Key;
                var below = (position.x, y: position.y + 1);

                // above room -> invalid
                if (IsHallway(position) && IsRoom(below))
                {
                    return false;
                }

                // wrong room or room contains others -> invalid
                if (IsRoom(position) && (position.x != TargetRoom(amphipod.type) || RoomContent(position.x).Any(t => t != amphipod.type)))
                {
                    return false;
                }

                // hallway to hallway -> invalid
                if (IsHallway(amphipod.position) && IsHallway(position))
                {
                    return false;
                }

                // position below in room is empty -> invalid
                if (IsRoom(position) && IsRoom(below) && !Amphipods.Any(a => a.position == below))
                {
                    return false;
                }

                return true;
            }).ToList();

            foreach ((var position, var steps) in validMoves)
            {
                var field = (Field)MemberwiseClone();
                field.Amphipods = new(field.Amphipods);

                field.Amphipods.Remove(amphipod);
                field.Amphipods.Add((amphipod.type, position));

                field.Energy += steps * StepEnergy(amphipod.type);

                field.Normalize();
                result.Add(field);
            }
        }

        return result;
    }

    public bool IsOrganized => Amphipods.All(a => IsRoom(a.position) && TargetRoom(a.type) == a.position.x);

    public void Normalize()
    {
        Amphipods = Amphipods
            .OrderBy(a => a.type)
            .ThenBy(a => a.position.x)
            .ThenBy(a => a.position.y)
            .ToList();
    }

    public bool Equals(Field? other)
    {
        if (other == null)
        {
            return false;
        }

        return Amphipods.SequenceEqual(other.Amphipods);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Field other)
        {
            return false;
        }

        return Equals(other);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var amphipod in Amphipods)
        {
            hash.Add(amphipod.type);
            hash.Add(amphipod.position);
        }

        return hash.ToHashCode();
    }

    public override string ToString()
    {
        var minX = Points.Min(p => p.x);
        var maxX = Points.Max(p => p.x);
        var minY = Points.Min(p => p.y);
        var maxY = Points.Max(p => p.y);

        var result = new StringBuilder();

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                var position = (x, y);

                var amphipod = Amphipods.FirstOrDefault(a => a.position == position, (type: Points.Contains(position) ? '.' : ' ', position));

                result.Append(amphipod.type);
            }

            result.AppendLine();
        }

        result.AppendLine($"energy: {Energy}");
        result.AppendLine();

        return result.ToString();
    }

    public static Field Parse(string[] input)
    {
        var result = new Field();

        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                var symbol = input[y][x];

                if (symbol == '.' || char.IsLetter(symbol))
                {
                    result.Points.Add((x, y));
                }

                if (char.IsLetter(symbol))
                {
                    result.Amphipods.Add((symbol, (x, y)));
                }
            }
        }

        result.Normalize();

        return result;
    }
}

class Program
{
    public static int MinimalEnergy(string[] input)
    {
        var start = Field.Parse(input);

        var current = new List<Field> { start };
        var seen = new HashSet<Field> { start };

        int minEnergy = int.MaxValue;

        while (current.Count > 0)
        {
            var next = current.SelectMany(f => f.Next()).Where(f => f.Energy < minEnergy).OrderBy(f => f.Energy).ToList();

            current.Clear();

            foreach (var field in next)
            {
                if (field.IsOrganized)
                {
                    minEnergy = Math.Min(minEnergy, field.Energy);
                    continue;
                }

                if (!seen.Contains(field))
                {
                    current.Add(field);
                    seen.Add(field);
                }
            }
        }

        return minEnergy;
    }

    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");

        var answer1 = MinimalEnergy(input);
        Console.WriteLine($"Answer 1: {answer1}");


        var parts = new List<string[]>
        {
            input[..3],
            new[] { "  #D#C#B#A#", "  #D#B#A#C#" },
            input[3..]
        };

        var answer2 = MinimalEnergy(parts.SelectMany(x => x).ToArray());
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
