const int size = 5;

int[,] ParseBoard(string[] lines)
{
    var board = new int[size, size];

    for (int row = 0; row < size; row++)
    {
        var values = lines[row].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

        for (int column = 0; column < size; column++)
        {
            board[row, column] = values[column];
        }
    }

    return board;
}

bool HasFullDimension(HashSet<int> chosen, Func<int, int, int> valueSelector)
{
    for (int i = 0; i < size; i++)
    {
        var full = true;

        for (int j = 0; j < size; j++)
        {
            full &= chosen.Contains(valueSelector(i, j));
        }

        if (full)
        {
            return true;
        }
    }

    return false;
}

bool HasFullRow(int[,] board, HashSet<int> chosen)
{
    return HasFullDimension(chosen, (i, j) => board[i, j]);
}

bool HasFullColumn(int[,] board, HashSet<int> chosen)
{
    return HasFullDimension(chosen, (i, j) => board[j, i]);
}

bool IsFull(int[,] board, HashSet<int> chosen)
{
    return HasFullRow(board, chosen) || HasFullColumn(board, chosen);
}

int FindMatch(List<int[,]> boards, List<int> numbers, bool first)
{
    HashSet<int> chosen = new();

    List<int[,]> candidates = new(boards);

    foreach (var number in numbers)
    {
        chosen.Add(number);

        if (!first && candidates.Count > 1)
        {
            candidates = candidates.Where(b => !IsFull(b, chosen)).ToList();
        }

        var match = candidates.FirstOrDefault(b => IsFull(b, chosen));

        if (match != null && (first || candidates.Count == 1))
        {
            var sum = match.OfType<int>().Where(x => !chosen.Contains(x)).Sum();

            return sum * number;
        }
    }

    throw new Exception();
}

var input = File.ReadAllLines("input.txt");

var numbers = input[0].Split(',').Select(int.Parse).ToList();

List<int[,]> boards = new();

for (int i = 2; i < input.Length; i += size + 1)
{
    var lines = input[i..(i + size)];

    boards.Add(ParseBoard(lines));
}

var answer1 = FindMatch(boards, numbers, true);

Console.WriteLine($"Answer 1: {answer1}");

var answer2 = FindMatch(boards, numbers, false);

Console.WriteLine($"Answer 2: {answer2}");
