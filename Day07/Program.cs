int Fuel1(int distance)
{
    return distance;
}

int Fuel2(int distance)
{
    return distance * (distance + 1) / 2;
}

int TotalFuel(Func<int, int> fuel, Dictionary<int, int> crabs, int position)
{
    return crabs.Sum(kv => fuel(Math.Abs(kv.Key - position)) * kv.Value);
}

var input = File.ReadAllText("input.txt");

var values = input
    .Split(',')
    .Select(int.Parse)
    .GroupBy(v => v)
    .ToDictionary(g => g.Key, g => g.Count());

var min = values.Min(kv => kv.Key);
var max = values.Max(kv => kv.Key);

var answer1 = Enumerable.Range(min, max).Min(p => TotalFuel(Fuel1, values, p));

Console.WriteLine($"Answer 1: {answer1}");

var answer2 = Enumerable.Range(min, max).Min(p => TotalFuel(Fuel2, values, p));

Console.WriteLine($"Answer 2: {answer2}");
