namespace TicTacToe.Core.Contracts;

/// <summary>
/// Represents the server decision for a restart request.
/// </summary>
/// <param name="UserMessage">The user-facing restart status message.</param>
/// <param name="Match">The fresh official snapshot.</param>
public sealed record RestartDecisionDto(string UserMessage, MatchSnapshotDto Match);