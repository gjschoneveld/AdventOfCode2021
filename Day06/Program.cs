Queue<long> createQueue(int size, Dictionary<long, int>? values = null)
{
    var queue = new Queue<long>(size);

    for (int i = 0; i < size; i++)
    {
        if (values != null && values.ContainsKey(i))
        {
            queue.Enqueue(values[i]);

            continue;
        }

        queue.Enqueue(0);
    }

    return queue;
}

long Simulate(Dictionary<long, int> values, int days)
{
    var regularSize = 7;
    var newSize = 2;

    var regularQueue = createQueue(regularSize, values);
    var newQueue = createQueue(newSize);

    for (int day = 1; day <= days; day++)
    {
        var zeros = regularQueue.Dequeue();
        var sevens = newQueue.Dequeue();

        regularQueue.Enqueue(zeros + sevens);
        newQueue.Enqueue(zeros);
    }

    return regularQueue.Sum() + newQueue.Sum();
}

var input = File.ReadAllText("input.txt");

var values = input
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(long.Parse)
    .GroupBy(v => v)
    .ToDictionary(g => g.Key, g => g.Count());

var answer1 = Simulate(values, 80);

Console.WriteLine($"Answer 1: {answer1}");

var answer2 = Simulate(values, 256);

Console.WriteLine($"Answer 2: {answer2}");
