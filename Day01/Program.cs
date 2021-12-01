int CountIncrements(List<int> values)
{
    return values
        .Skip(1)
        .Where((value, index) => value > values[index])
        .Count();
}

var input = File.ReadAllLines("input.txt");

var values = input.Select(int.Parse).ToList();

var answer1 = CountIncrements(values);

Console.WriteLine($"Answer 1: {answer1}");

var windowed = values
    .Skip(2)
    .Select((value, index) => value + values[index] + values[index + 1])
    .ToList();

var answer2 = CountIncrements(windowed);

Console.WriteLine($"Answer 2: {answer2}");
