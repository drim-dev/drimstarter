namespace Drimstarter.Common.Web.Endpoints;

public record PageResponse<T>(T[] Items, string? NextPageToken);
