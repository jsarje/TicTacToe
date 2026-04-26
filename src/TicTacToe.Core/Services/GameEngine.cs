using TicTacToe.Core.Models;

namespace TicTacToe.Core.Services;

/// <summary>
/// Evaluates moves and transitions for a local tic-tac-toe session.
/// </summary>
public sealed class GameEngine : IGameEngine
{
    private static readonly int[][] WinningLines =
    [
        [0, 1, 2],
        [3, 4, 5],
        [6, 7, 8],
        [0, 3, 6],
        [1, 4, 7],
        [2, 5, 8],
        [0, 4, 8],
        [2, 4, 6],
    ];

    /// <inheritdoc/>
    public GameSession CreateNewSession()
    {
        return GameSession.CreateNew();
    }

    /// <inheritdoc/>
    public GameSession PlayMove(GameSession session, int boardIndex)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (boardIndex is < 0 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(boardIndex), boardIndex, "Board index must be between 0 and 8.");
        }

        if (!session.IsBoardInteractive)
        {
            return session;
        }

        var selectedSpace = session.Board[boardIndex];
        if (selectedSpace.Mark is not null)
        {
            return session;
        }

        var updatedBoard = session.Board
            .Select(space => space.Index == boardIndex ? space with { Mark = session.CurrentPlayer } : space)
            .ToArray();

        var winningLine = GetWinningLine(updatedBoard);
        if (winningLine is not null)
        {
            return new GameSession(updatedBoard, session.CurrentPlayer, ToWinningResult(session.CurrentPlayer), winningLine);
        }

        if (updatedBoard.All(space => space.Mark is not null))
        {
            return new GameSession(updatedBoard, session.CurrentPlayer, GameResult.Draw, null);
        }

        return new GameSession(updatedBoard, SwitchPlayer(session.CurrentPlayer), GameResult.InProgress, null);
    }

    /// <inheritdoc/>
    public GameSession Restart(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        return CreateNewSession();
    }

    private static IReadOnlyList<int>? GetWinningLine(IReadOnlyList<BoardSpace> board)
    {
        foreach (var line in WinningLines)
        {
            var firstMark = board[line[0]].Mark;
            if (firstMark is null)
            {
                continue;
            }

            if (board[line[1]].Mark == firstMark && board[line[2]].Mark == firstMark)
            {
                return line;
            }
        }

        return null;
    }

    private static GameResult ToWinningResult(PlayerMark playerMark)
    {
        return playerMark switch
        {
            PlayerMark.X => GameResult.XWins,
            PlayerMark.O => GameResult.OWins,
            _ => throw new InvalidOperationException("Unknown player mark."),
        };
    }

    private static PlayerMark SwitchPlayer(PlayerMark currentPlayer)
    {
        return currentPlayer switch
        {
            PlayerMark.X => PlayerMark.O,
            PlayerMark.O => PlayerMark.X,
            _ => throw new InvalidOperationException("Unknown player mark."),
        };
    }
}