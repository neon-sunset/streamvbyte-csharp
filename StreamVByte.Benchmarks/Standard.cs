using BenchmarkDotNet.Attributes;

namespace StreamVByteImpl.Benchmarks;

[ShortRunJob]
[DisassemblyDiagnoser(maxDepth: 2, exportCombinedDisassemblyReport: true)]
public class Standard
{
    private static readonly uint[] Source = (0..1024).Select(i => (uint)i).ToArray();

    private static readonly byte[] Destination = new byte[4096];

    [Benchmark]
    public void Encode() => StreamVByte.Encode(Source, Destination);

    [Benchmark]
    public void Decode() => StreamVByte.Decode(Destination, Source);
}
