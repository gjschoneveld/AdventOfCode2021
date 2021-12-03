int MostCommon(List<int> bits)
{
    var ones = bits.Sum();

    return ones * 2 >= bits.Count ? 1 : 0;
}

int LeastCommon(List<int> bits)
{
    return 1 - MostCommon(bits);
}

string FindValue(string[] input, Func<List<int>, int> bitCriteria)
{
    var candidates = new List<string>(input);

    for (int i = 0; i < input[0].Length; i++)
    {
        var column = candidates.Select(x => x[i] - '0').ToList();

        var wanted = bitCriteria(column);

        candidates = candidates.Where(x => x[i] - '0' == wanted).ToList();

        if (candidates.Count == 1)
        {
            return candidates[0];
        }
    }

    throw new Exception();
}

var input = File.ReadAllLines("input.txt");

int gamma = 0;
int epsilon = 0;

for (int i = 0; i < input[0].Length; i++)
{
    var column = input.Select(x => x[i] - '0').ToList();

    gamma = gamma * 2 + MostCommon(column);
    epsilon = epsilon * 2 + LeastCommon(column);
}

var answer1 = gamma * epsilon;

Console.WriteLine($"Answer 1: {answer1}");


var oxygen = Convert.ToInt32(FindValue(input, MostCommon), 2);
var co2 = Convert.ToInt32(FindValue(input, LeastCommon), 2);

var answer2 = oxygen * co2;

Console.WriteLine($"Answer 2: {answer2}");
