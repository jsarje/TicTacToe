using TicTacToe.Core.Contracts;

namespace TicTacToe.Web.Client.Services;

/// <summary>
/// Provides client access to the server-authoritative match API.
/// </summary>
public interface IGameApiClient
{
    /// <summary>
    /// Loads the latest official match for the current browser.
    /// </summary>
    Task<MatchSnapshotDto> LoadMatchAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Submits one authoritative move.
    /// </summary>
    Task<MoveDecisionDto> SubmitMoveAsync(MoveRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests a fresh official match.
    /// </summary>
    Task<RestartDecisionDto> RestartAsync(CancellationToken cancellationToken = default);
}