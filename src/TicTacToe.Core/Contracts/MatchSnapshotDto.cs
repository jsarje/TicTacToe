using TicTacToe.Core.Models;

namespace TicTacToe.Core.Contracts;

/// <summary>
/// Represents the latest official match state returned to the client.
/// </summary>
/// <param name="MatchId">The persisted match identifier.</param>
/// <param name="Revision">The persisted optimistic concurrency revision.</param>
/// <param name="Board">The board in index order.</param>
/// <param name="CurrentPlayer">The active player while the match is in progress.</param>
/// <param name="Result">The official match result.</param>
/// <param name="WinningLine">The winning line when a player has won.</param>
/// <param name="IsBoardInteractive">A value indicating whether the board is interactive.</param>
/// <param name="StatusMessage">The user-facing status message.</param>
/// <param name="LastUpdatedUtc">The timestamp of the last accepted mutation.</param>
public sealed record MatchSnapshotDto(
    Guid MatchId,
    int Revision,
    IReadOnlyList<PlayerMark?> Board,
    PlayerMark CurrentPlayer,
    GameResult Result,
    IReadOnlyList<int>? WinningLine,
    bool IsBoardInteractive,
    string StatusMessage,
    DateTimeOffset LastUpdatedUtc);