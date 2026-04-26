namespace TicTacToe.Core.Tests;

public static class GameSessionTestData
{
    public static GameSession CreateSession(
        string board,
        PlayerMark currentPlayer = PlayerMark.X,
        GameResult result = GameResult.InProgress,
        IReadOnlyList<int>? winningLine = null)
    {
        if (board.Length != 9)
        {
            throw new ArgumentException("Board layouts must contain exactly nine characters.", nameof(board));
        }

        var spaces = board.Select((value, index) => new BoardSpace(index, ParseMark(value))).ToArray();
        return new GameSession(spaces, currentPlayer, result, winningLine);
    }

    private static PlayerMark? ParseMark(char value)
    {
        return value switch
        {
            'X' => PlayerMark.X,
            'O' => PlayerMark.O,
            '_' => null,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Only X, O, and _ are valid board markers."),
        };
    }
}