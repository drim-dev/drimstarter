namespace Drimstarter.Common.Utils;

public static class ByteUtils
{
    public static byte CalculateChecksum(this Span<byte> data)
    {
        byte checksum = 0;
        foreach (var b in data)
        {
            checksum ^= b;
        }

        return checksum;
    }
}
