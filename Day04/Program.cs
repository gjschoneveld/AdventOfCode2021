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

bool HasFullRow(int[,] board, HashSet<int> chosen)
{
    for (int row = 0; row < size; row++)
    {
        var full = true;

        for (int column = 0; column < size; column++)
        {
            full &= chosen.Contains(board[row, column]);
        }

        if (full)
        {
            return true;
        }
    }

    return false;
}

bool HasFullColumn(int[,] board, HashSet<int> chosen)
{
    for (int column = 0; column < size; column++)
    {
        var full = true;

        for (int row = 0; row < size; row++)
        {
            full &= chosen.Contains(board[row, column]);
        }

        if (full)
        {
            return true;
        }
    }

    return false;
}

int FirstMatch(List<int[,]> boards, List<int> numbers)
{
    HashSet<int> chosen = new();

    foreach (var number in numbers)
    {
        chosen.Add(number);

        var match = boards.FirstOrDefault(b => HasFullRow(b, chosen) || HasFullColumn(b, chosen));

        if (match != null)
        {
            var sum = match.OfType<int>().Where(x => !chosen.Contains(x)).Sum();

            return sum * number;
        }
    }

    throw new Exception();
}

int LastMatch(List<int[,]> boards, List<int> numbers)
{
    HashSet<int> chosen = new();

    List<int[,]> candidates = new(boards);

    foreach (var number in numbers)
    {
        chosen.Add(number);

        if (candidates.Count > 1)
        {
            candidates = candidates.Where(b => !HasFullRow(b, chosen) && !HasFullColumn(b, chosen)).ToList();
        }

        var match = candidates.FirstOrDefault(b => HasFullRow(b, chosen) || HasFullColumn(b, chosen));

        if (candidates.Count == 1 && match != null)
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

var answer1 = FirstMatch(boards, numbers);

Console.WriteLine($"Answer 1: {answer1}");

var answer2 = LastMatch(boards, numbers);

Console.WriteLine($"Answer 2: {answer2}");
