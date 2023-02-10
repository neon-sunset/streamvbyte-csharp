using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace StreamVByteImpl;

internal static class Intrinsics
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<uint> MultiplyLow(Vector128<uint> left, Vector128<uint> right) => Sse41.IsSupported
        ? Sse41.MultiplyLow(left, right)
        : AdvSimd.Multiply(left.AsInt32(), right.AsInt32()).AsUInt32();
}