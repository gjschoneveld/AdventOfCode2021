using System.Text.RegularExpressions;

(bool hit, int max) Simulate((int x, int y) velocity, (int x1, int x2, int y1, int y2) target)
{
    var max = 0;
    var position = (x: 0, y: 0);

    while (position.x <= target.x2 && position.y >= target.y1)
    {
        if (target.x1 <= position.x && position.x <= target.x2 &&
                target.y1 <= position.y && position.y <= target.y2)
        {
            return (true, max);
        }

        position = (x: position.x + velocity.x, y: position.y + velocity.y);
        velocity = (x: velocity.x > 0 ? velocity.x - 1 : 0, y: velocity.y - 1);

        max = Math.Max(max, position.y);
    }

    return (false, -1);
}

var input = File.ReadAllText("input.txt");

var matches = Regex.Matches(input, @"-?\d+");
var x1 = int.Parse(matches[0].Value);
var x2 = int.Parse(matches[1].Value);
var y1 = int.Parse(matches[2].Value);
var y2 = int.Parse(matches[3].Value);

var minX = 0;
var maxX = x2;

var minY = y1;
var maxY = -y1;

var max = 0;
var count = 0;

for (int y = minY; y <= maxY; y++)
{
    for (int x = minX; x <= maxX; x++)
    {
        var result = Simulate((x, y), (x1, x2, y1, y2));

        if (result.hit)
        {
            max = Math.Max(max, result.max);
            count++;
        }
    }
}

var answer1 = max;
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = count;
Console.WriteLine($"Answer 2: {answer2}");
