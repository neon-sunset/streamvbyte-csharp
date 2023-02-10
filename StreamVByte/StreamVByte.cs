using System.Runtime.Intrinsics;
using CommunityToolkit.Diagnostics;
namespace StreamVByteImpl;

public static class StreamVByte
{
    private const uint Shift = 1 | 1 << 9 | 1 << 18;
    private const uint Concat = 1 | 1 << 10 | 1 << 20 | 1 << 30;
    private const uint Sum = 1 | 1 << 8 | 1 << 16 | 1 << 24;

    private static ReadOnlySpan<byte> Ones => new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    private static ReadOnlySpan<uint> Shifts => new uint[] { Shift, Shift, Shift, Shift };
    private static ReadOnlySpan<byte> Lanecodes => new byte[]
    {
        0, 3, 2, 3,
        1, 3, 2, 3,
        128, 128, 128, 128,
        128, 128, 128, 128
    };
    private static ReadOnlySpan<byte> GatherHi => new byte[]
    {
        15, 11, 7, 3,
        15, 11, 7, 3,
        128, 128, 128, 128,
        128, 128, 128, 128
    };
    private static ReadOnlySpan<uint> Aggregators => new uint[] { Concat, Sum, 0 ,0 };

    public static int Encode(ReadOnlySpan<uint> source, Span<byte> destination)
    {
        Guard.HasSizeGreaterThan(source, 0);

        var shape = EncodedShape.FromLength(source.Length);

        var controlBytes = destination[..shape.ControlByteLength][..shape.ControlByteMaxLength];
        var encodedBytes = destination[shape.ControlByteLength..];

        return EncodeCore(source, controlBytes, encodedBytes)
            .ByteCount + controlBytes.Length;
    }

    public static int Decode(ReadOnlySpan<byte> source, Span<uint> destination)
    {
        var shape = EncodedShape.FromLength(destination.Length);

        var controlBytes = source[..shape.ControlByteLength][..shape.ControlByteMaxLength];
        var encodedBytes = source[shape.ControlByteLength..];

        return DecodeCore(controlBytes, encodedBytes, destination).NumDecoded;
    }

    private static (int NumCount, int ByteCount) EncodeCore(
        ReadOnlySpan<uint> source, Span<byte> controlBytes, Span<byte> encodedBytes)
    {
        var (numsEncoded, bytesEncoded) = (0, 0);

        var ones = Vector128.Create(Ones);
        var shifts = Vector128.Create(Shifts);
        var lanecodes = Vector128.Create(Lanecodes);
        var gatherHi = Vector128.Create(GatherHi);
        var aggregators = Vector128.Create(Aggregators);

        foreach (ref var controlByte in controlBytes)
        {
            var toEncode = Vector128.Create(source[numsEncoded..]);

            var mins = Vector128.Min(toEncode.AsByte(), ones);
            var bytemaps = Intrinsics
                .MultiplyLow(mins.AsUInt32(), shifts)
                .AsByte();
            var shuffledLanecodes = Vector128.Shuffle(lanecodes, bytemaps);
            var hiBytes = Vector128.Shuffle(shuffledLanecodes, gatherHi);
            var bytes = Intrinsics
                .MultiplyLow(hiBytes.AsUInt32(), aggregators)
                .AsByte();
            var code = bytes[3];
            var length = bytes[7] + 4;

            var encodeMask = Vector128.Create(Constants.EncodeTable[code]);
            var encoded = Vector128.Shuffle(toEncode.AsByte(), encodeMask);

            encoded.StoreUnsafe(ref encodedBytes[bytesEncoded]);

            controlByte = code;
            bytesEncoded += length;
            numsEncoded += 4;
        }

        return (numsEncoded, bytesEncoded);
    }

    private static (int NumDecoded, int BytesDecoded) DecodeCore(
        ReadOnlySpan<byte> controlBytes, ReadOnlySpan<byte> encodedBytes, Span<uint> destination)
    {
        var (bytesRead, numsDecoded) = (0, 0);

        foreach (var controlByte in controlBytes)
        {
            var length = Constants.DecodeTable[controlByte];
            var mask = Vector128.Create(Constants.DecodeShuffleTable[controlByte]);
            var data = Vector128.Create(encodedBytes[bytesRead..]);

            var decompressed = Vector128.Shuffle(data, mask).AsUInt32();

            decompressed.StoreUnsafe(ref destination[numsDecoded]);

            bytesRead += length;
            numsDecoded += 4;
        }

        return (bytesRead, numsDecoded);
    }

    // private static unsafe int EncodeQuad(Vector128<uint> data, byte* key, byte* data)
    // {
    //     var gatherLo = Vector128.Create(GatherLo);
    //     var aggregators = Vector128.Create(Aggregators);
    // }
}
