using Drimstarter.Common.Utils;
using SimpleBase;

namespace Drimstarter.Common.Database;

// TODO: write tests
public static class IdEncoding
{
    public static string Encode(long id)
    {
        Span<byte> bytes = BitConverter.GetBytes(id);
        Span<byte> bytesWithChecksum = new byte[bytes.Length + 1];
        bytes.CopyTo(bytesWithChecksum);
        bytesWithChecksum[^1] = bytes.CalculateChecksum();
        return Base32.Crockford.Encode(bytesWithChecksum).ToLower();
    }

    public static long Decode(string id)
    {
        var bytesWithChecksum = Base32.Crockford.Decode(id.ToUpper());
        var bytes = bytesWithChecksum.AsSpan()[..^1];
        var checksum = bytesWithChecksum[^1];
        var calculatedChecksum = bytes.CalculateChecksum();
        if (checksum != calculatedChecksum)
        {
            throw new ArgumentException("Checksum mismatch");
        }

        return BitConverter.ToInt64(bytes);
    }
}
