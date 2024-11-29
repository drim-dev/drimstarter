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

    public static bool TryDecode(string? encodedId, out long id)
    {
        id = default;

        if (string.IsNullOrWhiteSpace(encodedId))
        {
            return false;
        }

        byte[] bytesWithChecksum;
        try
        {
            bytesWithChecksum = Base32.Crockford.Decode(encodedId);
        }
        catch
        {
            return false;
        }

        var bytes = bytesWithChecksum.AsSpan()[..^1];
        var checksum = bytesWithChecksum[^1];
        var calculatedChecksum = bytes.CalculateChecksum();
        if (checksum != calculatedChecksum)
        {
            return false;
        }

        id = BitConverter.ToInt64(bytes);
        return true;
    }
}
