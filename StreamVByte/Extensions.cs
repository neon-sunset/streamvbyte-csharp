using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StreamVByteImpl;

static class Extensions
{
    internal unsafe static T* AsPtr<T>(this Span<T> source)
        where T : unmanaged => (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(source));

    internal unsafe static T* AsPtr<T>(this ReadOnlySpan<T> source)
        where T : unmanaged => (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(source));
}
