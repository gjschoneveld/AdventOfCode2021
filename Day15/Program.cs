int[,] Parse(string[] input)
{
    var result = new int[input.Length, input[0].Length];

    for (int row = 0; row < result.GetLength(0); row++)
    {
        for (int column = 0; column < result.GetLength(1); column++)
        {
            result[row, column] = input[row][column] - '0';
        }
    }

    return result;
}

bool IsValid(int[,] field, (int x, int y) position)
{
    return 0 <= position.x && position.x < field.GetLength(1) &&
        0 <= position.y && position.y < field.GetLength(0);
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

    return candidates.Where(p => IsValid(field, p)).ToList();
}

T GetValue<T>(T[,] field, (int x, int y) position)
{
    return field[position.y, position.x];
}

void SetValue<T>(T[,] field, (int x, int y) position, T value)
{
    field[position.y, position.x] = value;
}

int ToInt(int? value)
{
    if (value == null)
    {
        throw new Exception();
    }

    return value.Value;
}

int FindLowestTotalRisk(int[,] riskLevel)
{
    var totalRisk = new int?[riskLevel.GetLength(0), riskLevel.GetLength(1)];
    var visited = new bool[riskLevel.GetLength(0), riskLevel.GetLength(1)];

    var reachable = new HashSet<(int x, int y)> { (0, 0) };
    totalRisk[0, 0] = riskLevel[0, 0];

    var target = (x: riskLevel.GetLength(1) - 1, y: riskLevel.GetLength(0) - 1);

    while (true)
    {
        var position = reachable.MinBy(p => GetValue(totalRisk, p));

        if (position == target)
        {
            return ToInt(GetValue(totalRisk, target) - GetValue(totalRisk, (0, 0)));
        }

        reachable.Remove(position);
        SetValue(visited, position, true);

        var toVisit = GetNeighbours(riskLevel, position).Where(p => !GetValue(visited, p)).ToList();

        var total = ToInt(GetValue(totalRisk, position));

        foreach (var neighbour in toVisit)
        {
            var currentTotal = GetValue(totalRisk, neighbour);
            var neighbourRisk = GetValue(riskLevel, neighbour);

            var newTotal = total + neighbourRisk;

            if (currentTotal == null || newTotal < ToInt(currentTotal))
            {
                SetValue(totalRisk, neighbour, newTotal);
            }
        }

        reachable.UnionWith(toVisit);
    }
}

int[,] IncreaseValues(int[,] field, int amount)
{
    var result = new int[field.GetLength(0), field.GetLength(1)];

    for (int row = 0; row < result.GetLength(0); row++)
    {
        for (int column = 0; column < result.GetLength(1); column++)
        {
            result[row, column] = (field[row, column] + amount + 8) % 9 + 1;
        }
    }

    return result;
}

int[,] Expand(int[,] field, int size)
{
    var result = new int[size * field.GetLength(0), size * field.GetLength(1)];

    for (int y = 0; y < size; y++)
    {
        for (int x = 0; x < size; x++)
        {
            var section = IncreaseValues(field, x + y);

            for (int i = 0; i < section.GetLength(0); i++)
            {
                for (int j = 0; j < section.GetLength(1); j++)
                {
                    result[y * field.GetLength(0) + i, x * field.GetLength(1) + j] = section[i, j];
                }
            }
        }
    }

    return result;
}

var input = File.ReadAllLines("input.txt");
var riskLevel = Parse(input);

var answer1 = FindLowestTotalRisk(riskLevel);
Console.WriteLine($"Answer 1: {answer1}");

var expanded = Expand(riskLevel, 5);

var answer2 = FindLowestTotalRisk(expanded);
Console.WriteLine($"Answer 2: {answer2}");
