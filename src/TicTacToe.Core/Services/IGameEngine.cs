using TicTacToe.Core.Models;

namespace TicTacToe.Core.Services;

/// <summary>
/// Defines the operations that manage tic-tac-toe gameplay.
/// </summary>
public interface IGameEngine
{
    /// <summary>
    /// Creates a new empty game session.
    /// </summary>
    /// <returns>A new session with player X active.</returns>
    GameSession CreateNewSession();

    /// <summary>
    /// Attempts to record a move for the active player.
    /// </summary>
    /// <param name="session">The current session.</param>
    /// <param name="boardIndex">The zero-based board index to play.</param>
    /// <returns>The updated session, or the original session when the move is invalid.</returns>
    GameSession PlayMove(GameSession session, int boardIndex);

    /// <summary>
    /// Evaluates a move against the authoritative rules for the provided player.
    /// </summary>
    /// <param name="session">The current authoritative session.</param>
    /// <param name="boardIndex">The zero-based board index to play.</param>
    /// <param name="actingPlayer">The player attempting the move.</param>
    /// <returns>The move decision and resulting session state.</returns>
    GameMoveResult EvaluateMove(GameSession session, int boardIndex, PlayerMark actingPlayer);

    /// <summary>
    /// Resets the session to a new game.
    /// </summary>
    /// <param name="session">The current session.</param>
    /// <returns>A fresh game session.</returns>
    GameSession Restart(GameSession session);
}