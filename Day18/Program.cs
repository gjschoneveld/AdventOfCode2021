abstract class Vertex
{
    public InternalVertex? Parent { get; set; }

    public Vertex Add(Vertex number)
    {
        var sum = new InternalVertex(new List<Vertex> { this, number });
        sum.Reduce();

        return sum;
    }

    public abstract long Magnitude();

    public abstract bool Explode(int depth = 0);
    public abstract bool Split();

    public void Reduce()
    {
        var changeSeen = true;

        while (changeSeen)
        {
            changeSeen = Explode() || Split();
        }
    }

    private static char GetToken(string x, ref int position)
    {
        return x[position++];
    }

    private static void ConsumeToken(string x, ref int position, char token)
    {
        if (GetToken(x, ref position) != token)
        {
            throw new Exception();
        }
    }

    public static Vertex Parse(string x)
    {
        var position = 0;

        return Parse(x, ref position);
    }

    private static Vertex Parse(string x, ref int position)
    {
        var token = GetToken(x, ref position);

        if (token != '[')
        {
            return new LeafVertex { Value = int.Parse(token.ToString()) };
        }

        var left = Parse(x, ref position);
        ConsumeToken(x, ref position, ',');
        var right = Parse(x, ref position);
        ConsumeToken(x, ref position, ']');

        return new InternalVertex(new List<Vertex> { left, right });
    }
}

class LeafVertex : Vertex
{
    public long Value { get; set; }

    public override long Magnitude()
    {
        return Value;
    }

    public override bool Explode(int depth)
    {
        return false;
    }

    public override bool Split()
    {
        if (Value <= 9)
        {
            return false;
        }

        var left = new LeafVertex { Value = Value / 2 };
        var right = new LeafVertex { Value = (Value + 1) / 2 };

        var vertex = new InternalVertex(new List<Vertex> { left, right });

        Parent?.ReplaceChild(this, vertex);

        return true;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

class InternalVertex : Vertex
{
    private List<Vertex> Children { get; set; }

    public InternalVertex(List<Vertex> children)
    {
        foreach (var child in children)
        {
            child.Parent = this;
        }

        Children = children;
    }

    public override long Magnitude()
    {
        return 3 * Children[0].Magnitude() + 2 * Children[1].Magnitude();
    }

    public Vertex? FindSubtree(int side)
    {
        var node = this;
        var parent = Parent;

        while (parent != null)
        {
            if (node == parent.Children[1 - side])
            {
                return parent.Children[side];
            }

            node = parent;
            parent = node.Parent;
        }

        return null;
    }

    public LeafVertex? FindLeaf(int side)
    {
        var tree = FindSubtree(side);

        if (tree == null)
        {
            return null;
        }

        while (true)
        {
            if (tree is LeafVertex leafVertex)
            {
                return leafVertex;
            }

            if (tree is InternalVertex internalVertex)
            {
                tree = internalVertex.Children[1 - side];
            }
        }
    }

    public void ReplaceChild(Vertex old, Vertex replacement)
    {
        replacement.Parent = this;

        for (int i = 0; i < Children.Count; i++)
        {
            if (Children[i] == old)
            {
                Children[i] = replacement;
            }
        }
    }

    public override bool Explode(int depth)
    {
        if (depth < 4)
        {
            foreach (var child in Children)
            {
                var exploded = child.Explode(depth + 1);

                if (exploded)
                {
                    return true;
                }
            }

            return false;
        }

        var leaves = Children.Cast<LeafVertex>().ToList();

        for (int i = 0; i < Children.Count; i++)
        {
            var target = FindLeaf(i);

            if (target == null)
            {
                continue;
            }

            target.Value += leaves[i].Value;
        }

        Parent?.ReplaceChild(this, new LeafVertex());
        
        return true;
    }

    public override bool Split()
    {
        foreach (var child in Children)
        {
            var split = child.Split();

            if (split)
            {
                return true;
            }
        }

        return false;
    }

    public override string ToString()
    {
        return $"[{string.Join(",", Children.Select(v => v.ToString()))}]";
    }
}

class Program
{
    public static void Main()
    {
        var input = File.ReadAllLines("input.txt");
        var numbers = input.Select(Vertex.Parse).ToList();

        var sum = numbers.Aggregate((a, b) => a.Add(b));

        var answer1 = sum.Magnitude();
        Console.WriteLine($"Answer 1: {answer1}");


        var maxMagnitude = long.MinValue;

        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < input.Length; j++)
            {
                if (i == j)
                {
                    continue;
                }

                var left = Vertex.Parse(input[i]);
                var right = Vertex.Parse(input[j]);

                sum = left.Add(right);

                maxMagnitude = Math.Max(maxMagnitude, sum.Magnitude());
            }
        }

        var answer2 = maxMagnitude;
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
