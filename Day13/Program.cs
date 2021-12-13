(int x, int y) ParsePoint(string line)
{
    var parts = line.Split(',');

    return (int.Parse(parts[0]), int.Parse(parts[1]));
}

(char dimension, int value) ParseFold(string line)
{
    var parts = line.Split(new char[] { ' ', '=' });

    return (parts[2][0], int.Parse(parts[3]));
}

void Print(HashSet<(int x, int y)> paper)
{
    var minX = paper.Min(p => p.x);
    var maxX = paper.Max(p => p.x);

    var minY = paper.Min(p => p.y);
    var maxY = paper.Max(p => p.y);

    for (int y = minY; y <= maxY; y++)
    {
        for (int x = minX; x <= maxX; x++)
        {
            Console.Write(paper.Contains((x, y)) ? "█" : " ");
        }

        Console.WriteLine();
    }
}

void Fold(HashSet<(int x, int y)> paper, (char dimension, int value) fold)
{
    Func<(int x, int y), bool> predicate;
    Func<(int x, int y), (int x, int y)> map;

    if (fold.dimension == 'x')
    {
        predicate = p => p.x > fold.value;
        map = p => (2 * fold.value - p.x, p.y);
    }
    else
    {
        predicate = p => p.y > fold.value;
        map = p => (p.x, 2 * fold.value - p.y);
    }

    var mirrored = paper
        .Where(predicate)
        .Select(map)
        .ToList();

    paper.RemoveWhere(new(predicate));
    paper.UnionWith(mirrored);
}

var input = File.ReadAllLines("input.txt");

var pointLines = input.TakeWhile(line => line.Length > 0).ToList();
var foldLines = input[(pointLines.Count + 1)..].ToList();

var paper = pointLines.Select(ParsePoint).ToHashSet();
var folds = foldLines.Select(ParseFold).ToArray();

Fold(paper, folds[0]);

var answer1 = paper.Count;
Console.WriteLine($"Answer 1: {answer1}");

foreach (var fold in folds[1..])
{
    Fold(paper, fold);
}

Console.WriteLine("Answer 2:");
Print(paper);
