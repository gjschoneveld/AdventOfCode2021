using System.Text;

abstract class Packet
{
    public const int ValueType = 4;

    public const int SumType = 0;
    public const int ProductType = 1;
    public const int MinimumType = 2;
    public const int MaximumType = 3;
    public const int GreaterThanType = 5;
    public const int LessThanType = 6;
    public const int EqualToType = 7;

    public long Version { get; set; }
    public long Type { get; set; }

    public abstract long SumVersions();

    public abstract long Calculate();

    protected static long ParseValue(string bits, ref int position, int length)
    {
        var value = Convert.ToInt64(bits.Substring(position, length), 2);

        position += length;

        return value;
    }

    public static Packet Parse(string bits, ref int position)
    {
        var typePosition = position + 3;
        var type = ParseValue(bits, ref typePosition, 3);

        if (type == ValueType)
        {
            return ValuePacket.Parse(bits, ref position);
        }

        return OperatorPacket.Parse(bits, ref position);
    }
}

class ValuePacket : Packet
{
    public long Value { get; set; }

    public override long SumVersions()
    {
        return Version;
    }

    public override long Calculate()
    {
        return Value;
    }

    public static new Packet Parse(string bits, ref int position)
    {
        var version = ParseValue(bits, ref position, 3);
        var type = ParseValue(bits, ref position, 3);

        long value = 0;

        var last = false;

        while (!last)
        {
            last = ParseValue(bits, ref position, 1) == 0;

            var groupValue = ParseValue(bits, ref position, 4);
            value = (value << 4) + groupValue;
        }

        return new ValuePacket
        {
            Version = version,
            Type = type,
            Value = value
        };
    }
}

class OperatorPacket : Packet
{
    public List<Packet> SubPackets { get; set; } = new();

    public override long SumVersions()
    {
        return Version + SubPackets.Sum(p => p.SumVersions());
    }

    public override long Calculate()
    {
        var subValues = SubPackets.Select(p => p.Calculate()).ToList();

        switch (Type)
        {
            case SumType:
                return subValues.Sum();
            case ProductType:
                return subValues.Aggregate((a, b) => a * b);
            case MinimumType:
                return subValues.Min();
            case MaximumType:
                return subValues.Max();
            case GreaterThanType:
                return subValues[0] > subValues[1] ? 1 : 0;
            case LessThanType:
                return subValues[0] < subValues[1] ? 1 : 0;
            case EqualToType:
                return subValues[0] == subValues[1] ? 1 : 0;
        }

        throw new Exception();
    }

    public static new Packet Parse(string bits, ref int position)
    {
        var version = ParseValue(bits, ref position, 3);
        var type = ParseValue(bits, ref position, 3);
        var lengthType = ParseValue(bits, ref position, 1);

        var subPackets = new List<Packet>();

        if (lengthType == 0)
        {
            var length = ParseValue(bits, ref position, 15);
            var end = position + length;

            while (position < end)
            {
                var subPacket = Packet.Parse(bits, ref position);
                subPackets.Add(subPacket);
            }
        }
        else
        {
            var count = ParseValue(bits, ref position, 11);

            for (int i = 0; i < count; i++)
            {
                var subPacket = Packet.Parse(bits, ref position);
                subPackets.Add(subPacket);
            }
        }

        return new OperatorPacket
        {
            Version = version,
            Type = type,
            SubPackets = subPackets
        };
    }
}

class Program
{
    public static string ToBinary(string hex)
    {
        var bytes = Convert.FromHexString(hex);

        return string.Join("", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
    }

    public static void Main()
    {
        var input = File.ReadAllText("input.txt");
        var binary = ToBinary(input);

        var position = 0;
        var tree = Packet.Parse(binary, ref position);

        var answer1 = tree.SumVersions();
        Console.WriteLine($"Answer 1: {answer1}");

        var answer2 = tree.Calculate();
        Console.WriteLine($"Answer 2: {answer2}");
    }
}
