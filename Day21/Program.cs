int Modulo(int x, int mod)
{
    return (x - 1) % mod + 1;
}

List<int> DiceValues()
{
    var result = new List<int>();

    for (int dice1 = 1; dice1 <= 3; dice1++)
    {
        for (int dice2 = 1; dice2 <= 3; dice2++)
        {
            for (int dice3 = 1; dice3 <= 3; dice3++)
            {
                result.Add(dice1 + dice2 + dice3);
            }
        }
    }

    return result;
}

List<(long busy, long finished)> Play(int start, int goal)
{
    var wrapAround = 10;
    var diceValues = DiceValues();

    var result = new List<(long busy, long finished)> { (1, 0) };

    var busy = new Dictionary<(int position, int score), long>
    {
        [(start, 0)] = 1
    };

    while (busy.Count > 0)
    {
        var newBusy = new Dictionary<(int position, int score), long>();

        long finished = 0;

        foreach (((int position, int score), long count) in busy)
        {
            foreach (var value in diceValues)
            {
                var newPosition = Modulo(position + value, wrapAround);
                var newScore = score + newPosition;

                if (newScore >= goal)
                {
                    finished += count;
                    continue;
                }

                if (!newBusy.ContainsKey((newPosition, newScore)))
                {
                    newBusy[(newPosition, newScore)] = 0;
                }

                newBusy[(newPosition, newScore)] += count;
            }
        }

        busy = newBusy;
        result.Add((busy.Sum(kv => kv.Value), finished));
    }

    return result;
}

var input = File.ReadAllLines("input.txt");

var start = input.Select(x => x.Split(' ')[4]).Select(int.Parse).ToList();

var positions = start.ToList();
var scores = start.Select(p => 0).ToList();
var rounds = 0;
var dice = 1;
var rolls = 3;
var wrapAround = 10;
var player = 0;
var winScore = 1000;

while (scores.All(s => s < winScore))
{
    for (int i = 0; i < rolls; i++)
    {
        positions[player] += dice;
        dice++;
    }

    positions[player] = Modulo(positions[player], wrapAround);
    scores[player] += positions[player];

    player = (player + 1) % positions.Count;

    rounds++;
}

var losingScore = scores.First(s => s < winScore);

var answer1 = losingScore * rolls * rounds;
Console.WriteLine($"Answer 1: {answer1}");


var results = start.Select(p => Play(p, 21)).ToList();

var universes = new List<long> { 0, 0 };

for (int round = 1; round < results[0].Count; round++)
{
    universes[0] += results[0][round].finished * results[1][round - 1].busy;
    universes[1] += results[1][round].finished * results[0][round].busy;
}

var answer2 = universes.Max();
Console.WriteLine($"Answer 2: {answer2}");
