using System.Net.Http.Json;
using TicTacToe.Core.Contracts;

namespace TicTacToe.Web.Client.Services;

/// <summary>
/// Calls the same-origin authoritative match endpoints.
/// </summary>
public sealed class GameApiClient(HttpClient httpClient) : IGameApiClient
{
    /// <inheritdoc/>
    public async Task<MatchSnapshotDto> LoadMatchAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/match", cancellationToken);
        return await ReadRequiredContentAsync<MatchSnapshotDto>(response, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<MoveDecisionDto> SubmitMoveAsync(MoveRequestDto request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/match/moves", request, cancellationToken);
        return await ReadRequiredContentAsync<MoveDecisionDto>(response, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<RestartDecisionDto> RestartAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync("api/match/restart", null, cancellationToken);
        return await ReadRequiredContentAsync<RestartDecisionDto>(response, cancellationToken);
    }

    private static async Task<T> ReadRequiredContentAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
            return content ?? throw new GameApiException("The server returned an empty response.");
        }

        var problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(cancellationToken: cancellationToken);
        throw new GameApiException(problem?.Detail ?? "The official match is temporarily unavailable.");
    }

    private sealed record ApiProblemDetails(string? Detail);
}