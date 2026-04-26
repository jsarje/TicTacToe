namespace TicTacToe.Core.Models;

/// <summary>
/// Represents the complete state of a tic-tac-toe match.
/// </summary>
public sealed record GameSession
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameSession"/> class.
    /// </summary>
    /// <param name="board">The nine board spaces for the current session.</param>
    /// <param name="currentPlayer">The player whose turn is active.</param>
    /// <param name="result">The current session result.</param>
    /// <param name="winningLine">The winning line when the session is complete with a winner.</param>
    public GameSession(IReadOnlyList<BoardSpace> board, PlayerMark currentPlayer, GameResult result, IReadOnlyList<int>? winningLine)
    {
        ArgumentNullException.ThrowIfNull(board);

        if (board.Count != 9)
        {
            throw new ArgumentException("A game session must always contain exactly nine board spaces.", nameof(board));
        }

        Board = board.ToArray();
        CurrentPlayer = currentPlayer;
        Result = result;
        WinningLine = winningLine?.ToArray();
    }

    /// <summary>
    /// Gets the nine board spaces in index order.
    /// </summary>
    public IReadOnlyList<BoardSpace> Board { get; }

    /// <summary>
    /// Gets the player whose turn is active while the match is in progress.
    /// </summary>
    public PlayerMark CurrentPlayer { get; }

    /// <summary>
    /// Gets the current outcome of the match.
    /// </summary>
    public GameResult Result { get; }

    /// <summary>
    /// Gets the winning line indexes when the match has a winner.
    /// </summary>
    public IReadOnlyList<int>? WinningLine { get; }

    /// <summary>
    /// Gets the indexes that are still available for play.
    /// </summary>
    public IReadOnlyList<int> AvailableMoves => Board.Where(space => space.Mark is null).Select(space => space.Index).ToArray();

    /// <summary>
    /// Gets a value indicating whether the board can accept more input.
    /// </summary>
    public bool IsBoardInteractive => Result is GameResult.InProgress;

    /// <summary>
    /// Gets the user-facing status message for the current session.
    /// </summary>
    public string StatusMessage => Result switch
    {
        GameResult.InProgress => $"Player {CurrentPlayer} to move.",
        GameResult.XWins => "Player X wins!",
        GameResult.OWins => "Player O wins!",
        GameResult.Draw => "It's a draw.",
        _ => throw new InvalidOperationException("Unknown game result."),
    };

    /// <summary>
    /// Creates a new empty session with player X active.
    /// </summary>
    /// <returns>A fresh session.</returns>
    public static GameSession CreateNew()
    {
        return new GameSession(CreateBoard(), PlayerMark.X, GameResult.InProgress, null);
    }

    /// <summary>
    /// Determines whether a board index belongs to the winning line.
    /// </summary>
    /// <param name="boardIndex">The zero-based board index.</param>
    /// <returns><see langword="true"/> when the index is part of the winning line.</returns>
    public bool IsWinningSpace(int boardIndex)
    {
        return WinningLine?.Contains(boardIndex) is true;
    }

    private static IReadOnlyList<BoardSpace> CreateBoard()
    {
        return Enumerable.Range(0, 9)
            .Select(index => new BoardSpace(index, null))
            .ToArray();
    }
}