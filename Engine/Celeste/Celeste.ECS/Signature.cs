namespace Celeste.ECS;

public readonly struct Signature
    : IEquatable<Signature>
{
    private const int LongSizeInBits = sizeof(ulong) * 8;

    private readonly ulong[] _packedDatas;

    private Signature(ulong[] data)
    {
        _packedDatas = data;
    }

    public Signature(int sizeInBits)
    {
        var sizeInBitsOfULong = sizeof(ulong) * 8;
        var size = sizeInBits / sizeInBitsOfULong;
        _packedDatas = new ulong[size];
    }

    public void Flip(int index)
    {
        ref ulong packedData = ref GetPackedData(ref index);
        packedData ^= 1UL << index;
    }

    public void Clear()
    {
        var span = _packedDatas.AsSpan();
        foreach (ref ulong value in span)
        {
            value = 0UL;
        }
    }

    public bool IsBitSet(int index)
    {
        ref ulong packedData = ref GetPackedData(ref index);
        ulong mask = 1UL << index;
        return (packedData & mask) != 0;
    }

    private ref ulong GetPackedData(ref int index)
    {
        var temp = 0;
        if (index >= 64) temp = index >> 6;

        index -= temp * LongSizeInBits;
        return ref _packedDatas[temp];
    }

    public bool Equals(Signature other)
    {
        var span = _packedDatas.AsSpan();
        var otherSpan = other._packedDatas.AsSpan();

        if (span.Length != otherSpan.Length) return false;

        return span.SequenceEqual(otherSpan);
    }

    public override bool Equals(object? obj)
        => obj is Signature signature && Equals(signature);

    public override int GetHashCode()
    => _packedDatas.GetHashCode();

    public static bool operator ==(Signature left, Signature right)
        => left.Equals(right);

    public static bool operator !=(Signature left, Signature right)
        => !(left == right);

    public static Signature operator &(Signature left, Signature right)
    {
        if (left._packedDatas.Length != right._packedDatas.Length)
            return default;

        var data = new ulong[left._packedDatas.Length];
        for (int i = 0; i < left._packedDatas.Length; i++)
        {
            data[i] = left._packedDatas[i] & right._packedDatas[i];
        }

        return new(data);
    }
}