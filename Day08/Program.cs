var segments = new List<string>
{
    "abcefg", "cf", "acdeg", "acdfg", "bcdf", "abdfg", "abdefg", "acf", "abcdefg", "abcdfg"
};

(List<string> signals, List<string> output) Parse(string line)
{
    var parts = line.Split(new char[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries);

    return (new List<string>(parts[0..10]), new List<string>(parts[10..14]));
}

List<string> Permutations(string x)
{
    if (x.Length == 1)
    {
        return new List<string> { x };
    }

    var result = new List<string>();

    foreach (var c in x)
    {
        var remainder = x.Replace(c.ToString(), "");
        var inner = Permutations(remainder);

        result.AddRange(inner.Select(item => c + item));
    }

    return result;
}

string Map(string signal, string mapping)
{
    return new string(signal.Select(c => mapping[c - 'a']).OrderBy(c => c).ToArray());
}

string FindMapping(List<string> signals)
{
    return Permutations("abcdefg").First(m => signals.All(s => segments.Contains(Map(s, m))));
}

int MapOutput(List<string> output, string mapping)
{
    return output
        .Select(o => segments.IndexOf(Map(o, mapping)))
        .Aggregate((a, b) => 10 * a + b);
}

var input = File.ReadAllLines("input.txt");

var rules = input
    .Select(Parse)
    .ToList();

var wantedNumbers = new List<int> { 1, 4, 7, 8 };
var wantedLengths = wantedNumbers.Select(x => segments[x].Length).ToList();

var answer1 = rules.Sum(r => r.output.Count(o => wantedLengths.Contains(o.Length)));

Console.WriteLine($"Answer 1: {answer1}");

var answer2 = rules.Sum(r => MapOutput(r.output, FindMapping(r.signals)));

Console.WriteLine($"Answer 2: {answer2}");
