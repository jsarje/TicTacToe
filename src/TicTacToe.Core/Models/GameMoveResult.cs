namespace TicTacToe.Core.Models;

/// <summary>
/// Represents the result of evaluating one authoritative move request.
/// </summary>
/// <param name="Accepted">A value indicating whether the move changed the session.</param>
/// <param name="Session">The authoritative session after evaluation.</param>
/// <param name="RejectionReason">The reason a move was rejected, when applicable.</param>
public sealed record GameMoveResult(bool Accepted, GameSession Session, MoveRejectionReason? RejectionReason)
{
    /// <summary>
    /// Creates an accepted move result.
    /// </summary>
    /// <param name="session">The updated session.</param>
    /// <returns>An accepted result.</returns>
    public static GameMoveResult CreateAccepted(GameSession session)
    {
        return new GameMoveResult(true, session, null);
    }

    /// <summary>
    /// Creates a rejected move result.
    /// </summary>
    /// <param name="session">The unchanged authoritative session.</param>
    /// <param name="reason">The rejection reason.</param>
    /// <returns>A rejected result.</returns>
    public static GameMoveResult CreateRejected(GameSession session, MoveRejectionReason reason)
    {
        return new GameMoveResult(false, session, reason);
    }
}