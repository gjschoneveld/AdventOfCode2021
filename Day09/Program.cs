int[,] Parse(string[] lines)
{
    var result = new int[lines.Length, lines[0].Length];

    for (int row = 0; row < result.GetLength(0); row++)
    {
        for (int column = 0; column < result.GetLength(1); column++)
        {
            result[row, column] = lines[row][column] - '0';
        }
    }

    return result;
}

bool IsValidPosition(int[,] field, (int x, int y) position)
{
    return 0 <= position.x && position.x < field.GetLength(1) &&
        0 <= position.y && position.y < field.GetLength(0);
}

bool IsValidValue(int v)
{
    return v < 9;
}

int GetValue(int[,] field, (int x, int y) position)
{
    return field[position.y, position.x];
}

List<(int x, int y)> GetNeighbours(int[,] field, (int x, int y) position)
{
    var candidates = new List<(int x, int y)>
    {
        (position.x, position.y - 1),
        (position.x - 1, position.y),
        (position.x + 1, position.y),
        (position.x, position.y + 1),
    };

    return candidates.Where(p => IsValidPosition(field, p)).ToList();
}

List<(int x, int y)> FindLowestPoints(int[,] field)
{
    var result = new List<(int x, int y)>();

    for (int row = 0; row < field.GetLength(0); row++)
    {
        for (int column = 0; column < field.GetLength(1); column++)
        {
            var position = (x: column, y: row);

            if (GetValue(field, position) < GetNeighbours(field, position).Min(p => GetValue(field, p)))
            {
                result.Add(position);
            }
        }
    }

    return result;
}

int FindBasinSize(int[,] field, (int x, int y) position)
{
    var basin = new HashSet<(int x, int y)>();

    var newPositions = new List<(int x, int y)> { position };

    while (newPositions.Count > 0)
    {
        basin.UnionWith(newPositions);

        newPositions = newPositions
            .SelectMany(p => GetNeighbours(field, p))
            .Where(p => IsValidValue(GetValue(field, p)))
            .Where(p => !basin.Contains(p))
            .Distinct()
            .ToList();
    }

    return basin.Count;
}

var input = File.ReadAllLines("input.txt");

var field = Parse(input);

var lowest = FindLowestPoints(field);

var answer1 = lowest.Sum(p => GetValue(field, p) + 1);

Console.WriteLine($"Answer 1: {answer1}");

var basinSizes = lowest.Select(p => FindBasinSize(field, p));

var answer2 = basinSizes
    .OrderByDescending(s => s)
    .Take(3)
    .Aggregate((a, b) => a * b);

Console.WriteLine($"Answer 2: {answer2}");
