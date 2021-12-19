class Scanner
{
    public int Id { get; set; }
    public List<(int x, int y, int z)> Beacons { get; set; } = new();

    public static readonly List<Func<(int x, int y, int z), (int x, int y, int z)>> orientations = new()
    {
        p => (p.x, p.y, p.z),
        p => (p.x, -p.y, -p.z),
        p => (p.x, p.z, -p.y),
        p => (p.x, -p.z, p.y),
        p => (-p.x, p.z, p.y),
        p => (-p.x, -p.z, -p.y),
        p => (-p.x, p.y, -p.z),
        p => (-p.x, -p.y, p.z),

        p => (p.y, p.z, p.x),
        p => (p.y, -p.z, -p.x),
        p => (p.y, p.x, -p.z),
        p => (p.y, -p.x, p.z),
        p => (-p.y, p.x, p.z),
        p => (-p.y, -p.x, -p.z),
        p => (-p.y, p.z, -p.x),
        p => (-p.y, -p.z, p.x),

        p => (p.z, p.x, p.y),
        p => (p.z, -p.x, -p.y),
        p => (p.z, p.y, -p.x),
        p => (p.z, -p.y, p.x),
        p => (-p.z, p.y, p.x),
        p => (-p.z, -p.y, -p.x),
        p => (-p.z, p.x, -p.y),
        p => (-p.z, -p.x, p.y),
    };

    public static (int x, int y, int z) Add((int x, int y, int z) a, (int x, int y, int z) b)
    {
        return (a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static (int x, int y, int z) Subtract((int x, int y, int z) a, (int x, int y, int z) b)
    {
        return (a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public List<((int x, int y, int z) location, int relativeTo, int orientation)> Locations { get; set; } = new();

    public List<Scanner> PathToRoot { get; set; } = new();

    public (int x, int y, int z) InRootCoordinates((int x, int y, int z) location)
    {
        if (PathToRoot.Count == 0)
        {
            return location;
        }

        var scanner = this;

        foreach (var parent in PathToRoot)
        {
            var info = scanner.Locations.First(l => l.relativeTo == parent.Id);

            location = orientations[info.orientation](location);
            location = Add(location, info.location);

            scanner = parent;
        }

        return location;
    }

    public (int x, int y, int z) ScannerLocation()
    {
        if (PathToRoot.Count == 0)
        {
            return (0, 0, 0);
        }

        var parent = PathToRoot[0];
        var info = Locations.First(l => l.relativeTo == parent.Id);

        return parent.InRootCoordinates(info.location);
    }

    public List<(int x, int y, int z)> BeaconLocations()
    {
        return Beacons.Select(InRootCoordinates).ToList();
    }

    public int DistanceTo(Scanner other)
    {
        var difference = Subtract(ScannerLocation(), other.ScannerLocation());

        return Math.Abs(difference.x) + Math.Abs(difference.y) + Math.Abs(difference.z);
    }

    public void FindOverlap(Scanner other)
    {
        foreach (var otherBeacon in other.Beacons)
        {
            foreach (var myBeacon in Beacons)
            {
                for (int i = 0; i < orientations.Count; i++)
                {
                    var scannerLocation = Subtract(otherBeacon, orientations[i](myBeacon));
                    var beaconLocations = Beacons.Select(b => Add(scannerLocation, orientations[i](b))).ToList();

                    var overlap = other.Beacons.Intersect(beaconLocations).ToList();

                    if (overlap.Count >= 12)
                    {
                        Locations.Add((scannerLocation, other.Id, i));

                        return;
                    }
                }
            }
        }
    }

    public static void FindOverlap(Dictionary<int, Scanner> scanners)
    {
        foreach (var scanner1 in scanners.Values)
        {
            foreach (var scanner2 in scanners.Values)
            {
                if (scanner1 == scanner2)
                {
                    continue;
                }

                scanner1.FindOverlap(scanner2);
            }
        }
    }

    public static int FindMaxDistance(Dictionary<int, Scanner> scanners)
    {
        var maxDistance = 0;

        foreach (var scanner1 in scanners.Values)
        {
            foreach (var scanner2 in scanners.Values)
            {
                if (scanner1 == scanner2)
                {
                    continue;
                }

                var distance = scanner1.DistanceTo(scanner2);
                maxDistance = Math.Max(maxDistance, distance);
            }
        }

        return maxDistance;
    }

    public static void CreatePathsToRoot(Dictionary<int, Scanner> scanners)
    {
        var first = scanners.First().Value;

        var paths = new List<List<Scanner>> { new List<Scanner> { first } };

        var toVisit = new List<int> { first.Id };
        var visited = new HashSet<int> { first.Id };

        while (toVisit.Count > 0)
        {
            var nextVisit = new List<int>();

            foreach (var id in toVisit)
            {
                var neighbours = scanners[id]
                    .Locations
                    .Select(l => l.relativeTo)
                    .Where(x => !visited.Contains(x))
                    .ToList();

                visited.UnionWith(neighbours);
                nextVisit.AddRange(neighbours);

                var path = paths.First(p => p[0].Id == id);

                paths.AddRange(neighbours.Select(nb => path.Prepend(scanners[nb]).ToList()));
            }

            toVisit = nextVisit;
        }

        foreach (var path in paths)
        {
            scanners[path[0].Id].PathToRoot = path.Skip(1).ToList();
        }
    }

    public static void Initialize(Dictionary<int, Scanner> scanners)
    {
        FindOverlap(scanners);
        CreatePathsToRoot(scanners);
    }

    public static Scanner Parse(List<string> lines)
    {
        var headerParts = lines[0].Split(' ');
        var id = int.Parse(headerParts[2]);

        var beacons = lines
            .Skip(1)
            .Select(l => l.Split(',').Select(int.Parse).ToList())
            .Select(p => (p[0], p[1], p[2]))
            .ToList();

        return new Scanner
        {
            Id = id,
            Beacons = beacons
        };
    }
}

class Program
{
    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");

        var scanners = new Dictionary<int, Scanner>();

        var start = 0;

        for (int end = 0; end < input.Length + 1; end++)
        {
            if (end >= input.Length || input[end] == "")
            {
                var lines = input[start..end].ToList();
                var scanner = Scanner.Parse(lines);
                scanners.Add(scanner.Id, scanner);

                start = end + 1;
            }
        }

        Scanner.Initialize(scanners);

        var beacons = scanners.Values.SelectMany(s => s.BeaconLocations()).ToHashSet();

        var answer1 = beacons.Count;
        Console.WriteLine($"Answer 1: {answer1}");

        var answer2 = Scanner.FindMaxDistance(scanners);
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
