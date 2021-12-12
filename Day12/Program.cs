void AddVertex(Dictionary<string, List<string>> graph, string from, string to)
{
    if (from == "end" || to == "start")
    {
        return;
    }

    if (!graph.ContainsKey(from))
    {
        graph[from] = new List<string>();
    }

    graph[from].Add(to);
}

Dictionary<string, List<string>> Parse(string[] lines)
{
    Dictionary<string, List<string>> graph = new();

    foreach (string line in lines)
    {
        var nodes = line.Split('-');

        AddVertex(graph, nodes[0], nodes[1]);
        AddVertex(graph, nodes[1], nodes[0]);
    }

    return graph;
}

bool isSmallCave(string node)
{
    return char.IsLower(node[0]);
}

int CountPaths(Dictionary<string, List<string>> graph, List<string> path, bool part2)
{
    var currentNode = path.Last();

    if (currentNode == "end")
    {
        return 1;
    }

    var smallCaveAllowed = part2 && !path.Where(isSmallCave).GroupBy(n => n).Any(g => g.Count() > 1);

    var toVisit = graph[currentNode].Where(n => !isSmallCave(n) || smallCaveAllowed || !path.Contains(n)).ToList();

    var paths = 0;

    foreach (var nextNode in toVisit)
    {
        var nextPath = path.Append(nextNode).ToList();

        paths += CountPaths(graph, nextPath, part2);
    }

    return paths;
}

var input = File.ReadAllLines("input.txt");
var graph = Parse(input);

var answer1 = CountPaths(graph, new List<string> { "start" }, false);

Console.WriteLine($"Answer 1: {answer1}");

var answer2 = CountPaths(graph, new List<string> { "start" }, true);

Console.WriteLine($"Answer 2: {answer2}");
