(string from, char to) ParseRule(string x)
{
    var parts = x.Split(new[] { ' ', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);

    return (parts[0], parts[1][0]);
}

Dictionary<string, long> ConvertToPairs(string x)
{
    var pairs = new Dictionary<string, long>();

    for (int i = 0; i < x.Length - 1; i++)
    {
        var pair = x.Substring(i, 2);

        if (!pairs.ContainsKey(pair))
        {
            pairs[pair] = 0;
        }

        pairs[pair]++;
    }

    return pairs;
}

Dictionary<string, long> Step(Dictionary<string, char> rules, Dictionary<string, long> pairs)
{
    var result = new Dictionary<string, long>();

    foreach ((var pair, var count) in pairs)
    {
        var middle = rules[pair];

        var newPairs = new List<string>
        {
            new string(new[] { pair[0], middle }),
            new string(new[] { middle, pair[1] })
        };

        foreach (var newPair in newPairs)
        {
            if (!result.ContainsKey(newPair))
            {
                result[newPair] = 0;
            }

            result[newPair] += count;
        }
    }

    return result;
}

Dictionary<char, long> CountElements(Dictionary<string, long> pairs, char start, char end)
{
    var counts = new Dictionary<char, long>();

    foreach ((var pair, var count) in pairs)
    {
        foreach (var element in pair)
        {
            if (!counts.ContainsKey(element))
            {
                counts[element] = 0;
            }

            counts[element] += count;
        }
    }

    // start and end are counted once, everthing else twice -> make sure everthing is counted twice 
    counts[start]++;
    counts[end]++;

    // remove duplication
    counts = counts.ToDictionary(kv => kv.Key, kv => kv.Value / 2);    

    return counts;
}

long Simulate(Dictionary<string, char> rules, string start, int steps)
{
    var pairs = ConvertToPairs(start);

    for (int step = 1; step <= steps; step++)
    {
        pairs = Step(rules, pairs);
    }

    var counts = CountElements(pairs, start[0], start[^1]);

    var min = counts.Min(kv => kv.Value);
    var max = counts.Max(kv => kv.Value);

    return max - min;
}

var input = File.ReadAllLines("input.txt");

var x = input[0];
var rules = input[2..].Select(ParseRule).ToDictionary(r => r.from, r => r.to);

var answer1 = Simulate(rules, x, 10);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = Simulate(rules, x, 40);
Console.WriteLine($"Answer 2: {answer2}");
