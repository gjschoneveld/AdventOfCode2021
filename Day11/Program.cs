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

List<(int x, int y)> GetNeighbours(int[,] field, (int x, int y) position)
{
    var candidates = new List<(int x, int y)>
    {
        (position.x - 1, position.y - 1),
        (position.x, position.y - 1),
        (position.x + 1, position.y - 1),
        (position.x - 1, position.y),
        (position.x + 1, position.y),
        (position.x - 1, position.y + 1),
        (position.x, position.y + 1),
        (position.x + 1, position.y + 1),
    };

    return candidates.Where(p => IsValidPosition(field, p)).ToList();
}

int Step(int[,] field)
{
    List<(int x, int y)> toIncrease = new();

    for (int row = 0; row < field.GetLength(0); row++)
    {
        for (int column = 0; column < field.GetLength(1); column++)
        {
            toIncrease.Add((x: column, y: row));
        }
    }

    HashSet<(int x, int y)> flashed = new();

    while (toIncrease.Count > 0)
    {
        foreach (var position in toIncrease)
        {
            field[position.y, position.x]++;
        }

        int flashLimit = 9;
        var toFlash = toIncrease.Where(p => field[p.y, p.x] > flashLimit).Distinct().ToList();

        foreach (var position in toFlash)
        {
            field[position.y, position.x] = 0;
            flashed.Add(position);
        }

        toIncrease = toFlash.SelectMany(p => GetNeighbours(field, p)).Where(p => !flashed.Contains(p)).ToList();
    }

    return flashed.Count;
}

var input = File.ReadAllLines("input.txt");
var field = Parse(input);

int step;
int total = 0;

for (step = 1; step <= 100; step++)
{
    total += Step(field);
}

var answer1 = total;

Console.WriteLine($"Answer 1: {answer1}");


field = Parse(input);

step = 1;

while (true)
{
    var flashes = Step(field);

    if (flashes == field.Length)
    {
        break;
    }

    step++;
}

var answer2 = step;

Console.WriteLine($"Answer 2: {answer2}");
