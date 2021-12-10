long CorruptValue(int index)
{
    var values = new[] { 3, 57, 1197, 25137 };

    return values[index];
}

long IncompleteValue(IEnumerable<int> indices)
{
    return indices.Aggregate(0L, (subtotal, index) => subtotal * 5 + index + 1);
}

(bool corrupt, long value) ProcessLine(string line)
{
    var openSymbols = new List<char> { '(', '[', '{', '<' };
    var closeSymbols = new List<char> { ')', ']', '}', '>' };

    var stack = new Stack<int>();

    foreach (var c in line)
    {
        if (openSymbols.Contains(c))
        {
            stack.Push(openSymbols.IndexOf(c));
            continue;
        }

        var openIndex = stack.Pop();
        var closeIndex = closeSymbols.IndexOf(c);

        if (openIndex != closeIndex)
        {
            return (true, CorruptValue(closeIndex));
        }
    }

    return (false, IncompleteValue(stack));
}

var input = File.ReadAllLines("input.txt");

var results = input.Select(ProcessLine).ToList();

var answer1 = results
    .Where(r => r.corrupt)
    .Select(r => r.value)
    .Aggregate((sum, v) => sum + v);

Console.WriteLine($"Answer 1: {answer1}");

var incompleteValues = results
    .Where(r => !r.corrupt)
    .Select(r => r.value)
    .ToList();

var answer2 = incompleteValues
    .OrderBy(v => v)
    .Skip(incompleteValues.Count / 2)
    .First();

Console.WriteLine($"Answer 2: {answer2}");
