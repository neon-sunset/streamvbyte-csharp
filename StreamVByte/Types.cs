using System.Runtime.CompilerServices;

namespace StreamVByteImpl;

readonly record struct EncodedShape(
    int ControlByteLength,
    int ControlByteMaxLength,
    int RemainderLength)
{
    public static EncodedShape FromLength(int length) => new(
        ControlByteLength: (length + 3) / 4,
        ControlByteMaxLength: length / 4,
        RemainderLength: length % 4);
}

readonly ref struct SplitPair<T>
{
    public readonly Span<T> Left;

    public readonly Span<T> Right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SplitPair(Span<T> left, Span<T> right)
    {
        Left = left;
        Right = right;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out Span<T> left, out Span<T> right)
    {
        left = Left;
        right = Right;
    }
}

public readonly ref struct ReadOnlySplitPair<T>
{
    public readonly ReadOnlySpan<T> Left;

    public readonly ReadOnlySpan<T> Right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySplitPair(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        Left = left;
        Right = right;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out ReadOnlySpan<T> left, out ReadOnlySpan<T> right)
    {
        left = Left;
        right = Right;
    }
}