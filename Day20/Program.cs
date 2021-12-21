HashSet<(int x, int y)> Parse(string[] lines)
{
    var result = new HashSet<(int x, int y)>();

    for (int row = 0; row < lines.Length; row++)
    {
        for (int column = 0; column < lines[row].Length; column++)
        {
            if (lines[row][column] == '#')
            {
                result.Add((column, row));
            }
        }
    }

    return result;
}

List<(int x, int y)> Square((int x, int y) center)
{
    return new List<(int x, int y)>
    {
        (center.x - 1, center.y - 1),
        (center.x, center.y - 1),
        (center.x + 1, center.y - 1),
        (center.x - 1, center.y),
        (center.x, center.y),
        (center.x + 1, center.y),
        (center.x - 1, center.y + 1),
        (center.x, center.y + 1),
        (center.x + 1, center.y + 1),
    };
}

HashSet<(int x, int y)> EnhanceImage(HashSet<(int x, int y)> image, List<bool> algorithm, bool allElseLit, int minX, int maxX, int minY, int maxY)
{
    var result = new HashSet<(int x, int y)>();

    for (int x = minX - 1; x <= maxX + 1; x++)
    {
        for (int y = minY - 1; y <= maxY + 1; y++)
        {
            var number = Square((x, y)).Select(p =>
            {
                if (minX <= p.x && p.x <= maxX && minY <= p.y && p.y <= maxY)
                {
                    return image.Contains(p);
                }

                return allElseLit;

            }).Select(v => v ? 1 : 0).Aggregate((a, b) => 2 * a + b);

            if (algorithm[number])
            {
                result.Add((x, y));
            }
        }
    }

    return result;
}

int Simulate(HashSet<(int x, int y)> image, List<bool> algorithm, bool flickerAllElse, int count)
{
    var allElseLit = false;

    var minX = image.Min(p => p.x);
    var maxX = image.Max(p => p.x);

    var minY = image.Min(p => p.y);
    var maxY = image.Max(p => p.y);

    for (int i = 0; i < count; i++)
    {
        image = EnhanceImage(image, algorithm, allElseLit, minX--, maxX++, minY--, maxY++);

        if (flickerAllElse)
        {
            allElseLit = !allElseLit;
        }
    }

    return image.Count;
}

var input = File.ReadAllLines("input.txt");

var algorithm = input[0].Select(c => c == '#').ToList();
var image = Parse(input[2..]);

var flicker = algorithm[0] && !algorithm[511];

var answer1 = Simulate(image, algorithm, flicker, 2);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = Simulate(image, algorithm, flicker, 50);
Console.WriteLine($"Answer 2: {answer2}");
