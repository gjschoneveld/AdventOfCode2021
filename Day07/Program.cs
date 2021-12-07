int TotalFuelMethod1(Dictionary<int, int> crabs, int position)
{
    return crabs.Sum(kv => Math.Abs(kv.Key - position) * kv.Value);
}

int FuelMethod2(int distance)
{
    return distance * (distance + 1) / 2;
}

int TotalFuelMethod2(Dictionary<int, int> crabs, int position)
{
    return crabs.Sum(kv => FuelMethod2(Math.Abs(kv.Key - position)) * kv.Value);
}

var input = File.ReadAllText("input.txt");

var values = input
    .Split(',')
    .Select(int.Parse)
    .GroupBy(v => v)
    .ToDictionary(g => g.Key, g => g.Count());

var min = values.Min(kv => kv.Key);
var max = values.Max(kv => kv.Key);

var answer1 = Enumerable.Range(min, max).Min(p => TotalFuelMethod1(values, p));

Console.WriteLine($"Answer 1: {answer1}");

var answer2 = Enumerable.Range(min, max).Min(p => TotalFuelMethod2(values, p));

Console.WriteLine($"Answer 2: {answer2}");
