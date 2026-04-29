using TicTacToe.Core.Models;

namespace TicTacToe.Web.Infrastructure;

/// <summary>
/// Builds user-facing messages for authoritative move decisions.
/// </summary>
public static class MoveDecisionMessageFactory
{
    public static string ForAcceptedMove(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        return session.StatusMessage;
    }

    public static string ForRejection(MoveRejectionReason reason, int? spaceIndex = null)
    {
        return reason switch
        {
            MoveRejectionReason.OccupiedSquare => $"Square {(spaceIndex ?? 0) + 1} is already occupied.",
            MoveRejectionReason.WrongTurn => "That move was rejected because the official match has a different active player.",
            MoveRejectionReason.MatchComplete => "This match is already complete. Restart to begin a new game.",
            MoveRejectionReason.StaleRevision => "Your board was out of date. The latest official state has been reloaded.",
            _ => "The move could not be applied.",
        };
    }

    public static string ForRestart()
    {
        return "New game started. Player X to move.";
    }
}