using System.Text.Json;
using Drimstarter.Common.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleBase;

namespace Drimstarter.Common.Database;

public class Paging
{
    private readonly byte[] _encryptionKey;
    private readonly byte[] _iv;
    private readonly int _defaultMaxPageSize;
    private readonly int _maxMaxPageSize;

    public Paging(IOptions<PagingOptions> options)
    {
        _encryptionKey = Convert.FromBase64String(options.Value.TokenEncryptionKeyInBase64);
        _iv = Convert.FromBase64String(options.Value.TokenIvInBase64);
        _defaultMaxPageSize = options.Value.DefaultMaxPageSize;
        _maxMaxPageSize = options.Value.MaxMaxPageSize;
    }

    public bool TryGetMaxPageSize(int? originalMaxPageSize, out int maxPageSize)
    {
        maxPageSize = originalMaxPageSize ?? _defaultMaxPageSize;
        if (maxPageSize < 1)
        {
            return false;
        }

        return maxPageSize <= _maxMaxPageSize;
    }

    public bool TryGetOffsetAndLimit(string? pageToken, int maxPageSize, out int? offset, out int? limit,
        params object?[] queryParams)
    {
        if (string.IsNullOrEmpty(pageToken))
        {
            offset = 0;
            limit = maxPageSize;
            return true;
        }

        offset = null;
        limit = null;

        var pageTokenBytes = Base32.Crockford.Decode(pageToken);
        if (!CryptoUtils.TryDecryptAes(pageTokenBytes, _encryptionKey, _iv, out var decryptedPageToken))
        {
            return false;
        }

        PageToken? pageTokenObject;
        try
        {
            pageTokenObject = JsonSerializer.Deserialize<PageToken>(decryptedPageToken!);
        }
        catch
        {
            return false;
        }

        if (pageTokenObject!.QueryHash != HashQueryParams(queryParams))
        {
            return false;
        }

        offset = pageTokenObject.Offset;
        limit = maxPageSize;
        return true;
    }

    public string? CreateNextPageToken(int count, int offset, int limit, params object?[] queryParams)
    {
        if (count < limit)
        {
            return null;
        }

        var pageToken = new PageToken(offset + limit, HashQueryParams(queryParams));

        var pageTokenString = JsonSerializer.Serialize(pageToken);

        var cipherTextBytes = CryptoUtils.EncryptAes(pageTokenString, _encryptionKey, _iv);
        return Base32.Crockford.Encode(cipherTextBytes);
    }

    private static int HashQueryParams(params object?[] queryParams)
    {
        var queryParamString = string.Join(":", queryParams);
        return HashUtils.HashSha256(queryParamString);
    }

    private record PageToken(int Offset, int QueryHash);
}

public class PagingOptions
{
    public required string TokenEncryptionKeyInBase64 { get; init; }
    public required string TokenIvInBase64 { get; init; }
    public required int DefaultMaxPageSize { get; init; } = 10;
    public required int MaxMaxPageSize { get; init; } = 100;
}

public static class Extensions
{
    public static IServiceCollection AddPaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PagingOptions>(configuration.GetSection("Database:Paging"));

        services.AddSingleton<Paging>();

        return services;
    }
}
