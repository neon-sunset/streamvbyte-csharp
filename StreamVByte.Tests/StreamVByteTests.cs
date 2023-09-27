namespace StreamVByteImpl.Tests;

public class StreamVByteTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)] // fails
    [InlineData(16)]
    [InlineData(17)] // fails
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(128)]
    [InlineData(256)]
    [InlineData(512)]
    [InlineData(513)] // fails
    public void CanRoundtrip(uint count)
    {
        var source = new uint[count];

        for (uint i = 0; i < count; i++)
        {
            source[i] = i;
        }

        byte[] output = new byte[source.Length * 4];

        StreamVByte.Encode(source, output);

        var decoded = new uint[source.Length];

        StreamVByte.Decode(output, decoded);

        Assert.Equal(source, decoded);
    }
}
