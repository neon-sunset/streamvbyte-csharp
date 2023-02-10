using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace StreamVByteImpl;

static class Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector128<uint> AddSaturate(Vector128<ushort> left, Vector128<ushort> right) =>
        Sse2.IsSupported
            ? Sse2.AddSaturate(left, right).AsUInt32()
            : AdvSimd.AddSaturate(left, right).AsUInt32();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector128<byte> PackUnsignedSaturate<T>(Vector128<T> left, Vector128<T> right)
        where T : unmanaged
    {
        return Sse2.IsSupported
            ? Sse2.PackUnsignedSaturate(left.AsInt16(), right.AsInt16())
            : AdvSimd.ExtractNarrowingSaturateUpper(
                AdvSimd.ExtractNarrowingSaturateLower(left.AsUInt16()), right.AsUInt16());
    }
}