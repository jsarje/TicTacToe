using TicTacToe.Core.Contracts;

namespace TicTacToe.Web.Infrastructure;

/// <summary>
/// Defines the authoritative application boundary for loading, moving, and restarting a match.
/// </summary>
public interface IMatchRefereeService
{
    /// <summary>
    /// Loads the latest official match for the current browser.
    /// </summary>
    Task<MatchSnapshotDto> LoadMatchAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Applies one move against the official match state.
    /// </summary>
    Task<MoveDecisionDto> ApplyMoveAsync(MoveRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Replaces the current match with a fresh official session.
    /// </summary>
    Task<RestartDecisionDto> RestartAsync(CancellationToken cancellationToken);
}