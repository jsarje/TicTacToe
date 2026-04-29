using TicTacToe.Core.Models;

namespace TicTacToe.Core.Contracts;

/// <summary>
/// Represents the server decision for a move attempt.
/// </summary>
/// <param name="Accepted">A value indicating whether the move was persisted.</param>
/// <param name="RejectionReason">The gameplay rejection reason, when applicable.</param>
/// <param name="UserMessage">The user-facing status or error message.</param>
/// <param name="Match">The latest official snapshot.</param>
public sealed record MoveDecisionDto(bool Accepted, MoveRejectionReason? RejectionReason, string UserMessage, MatchSnapshotDto Match);