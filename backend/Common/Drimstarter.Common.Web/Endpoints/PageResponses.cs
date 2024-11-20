namespace Drimstarter.Common.Web.Endpoints;

public record PageResponse<T>(T[] Items, int TotalItems);

public record OffsetLimitResponse<T>(T[] Items, bool HasMore);

public record KeysetResponse<T>(T[] Items, string? NextStartAfter);

public record PageTokenResponse<T>(T[] Items, string? NextPageToken);
