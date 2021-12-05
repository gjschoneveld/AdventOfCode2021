class Line
{
    public (int x, int y) From { get; set; }
    public (int x, int y) To { get; set; }

    public IEnumerable<(int x, int y)> StraightPoints()
    {
        if (From.x == To.x)
        {
            var from = Math.Min(From.y, To.y);
            var to = Math.Max(From.y, To.y);

            for (int y = from; y <= to; y++)
            {
                yield return (From.x, y);
            }
        }
        
        if (From.y == To.y)
        {
            var from = Math.Min(From.x, To.x);
            var to = Math.Max(From.x, To.x);

            for (int x = from; x <= to; x++)
            {
                yield return (x, From.y);
            }
        }
    }

    public IEnumerable<(int x, int y)> DiagonalPoints()
    {
        int fromX = From.x;
        int toX = To.x;

        int y = From.y;
        int incrementY = From.y < To.y ? 1 : -1;

        if (From.x > To.x)
        {
            fromX = To.x;
            toX = From.x;
            y = To.y;
            incrementY = To.y < From.y ? 1 : -1;
        }

        for (int x = fromX; x <= toX; x++, y += incrementY)
        {
            yield return (x, y);
        }
    }

    public IEnumerable<(int x, int y)> AllPoints()
    {
        if (From.x == To.x || From.y == To.y)
        {
            return StraightPoints();
        }

        return DiagonalPoints();
    }

    public static Line Parse(string x)
    {
        var parts = x.Split(new[] { ' ', ',', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);

        return new Line
        {
            From = (int.Parse(parts[0]), int.Parse(parts[1])),
            To = (int.Parse(parts[2]), int.Parse(parts[3])),
        };
    }
}

class Program
{
    public static int FillGrid(List<Line> lines, Func<Line, IEnumerable<(int x, int y)>> pointsSelector)
    {
        Dictionary<(int x, int y), int> grid = new();

        foreach (var line in lines)
        {
            foreach (var point in pointsSelector(line))
            {
                if (!grid.ContainsKey(point))
                {
                    grid[point] = 0;
                }

                grid[point]++;
            }
        }

        return grid.Values.Count(x => x > 1);
    }

    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");
        var lines = input.Select(Line.Parse).ToList();

        var answer1 = FillGrid(lines, line => line.StraightPoints());

        Console.WriteLine($"Answer 1: {answer1}");

        var answer2 = FillGrid(lines, line => line.AllPoints());

        Console.WriteLine($"Answer 2: {answer2}");
    }
}
