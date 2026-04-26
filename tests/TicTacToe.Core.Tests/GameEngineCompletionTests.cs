namespace TicTacToe.Core.Tests;

public sealed class GameEngineCompletionTests
{
    private readonly GameEngine gameEngine = new();

    [Fact]
    public void PlayMove_ShouldDetectWinningLine()
    {
        // Arrange
        var session = GameSessionTestData.CreateSession("XX_OO____", PlayerMark.X);

        // Act
        var completedSession = gameEngine.PlayMove(session, 2);

        // Assert
        completedSession.Result.Should().Be(GameResult.XWins);
        completedSession.WinningLine.Should().Equal(0, 1, 2);
        completedSession.StatusMessage.Should().Be("Player X wins!");
    }

    [Fact]
    public void PlayMove_ShouldDetectDraw()
    {
        // Arrange
        var session = GameSessionTestData.CreateSession("XOXOOXX__", PlayerMark.X);
        session = gameEngine.PlayMove(session, 7);

        // Act
        var completedSession = gameEngine.PlayMove(session, 8);

        // Assert
        completedSession.Result.Should().Be(GameResult.Draw);
        completedSession.WinningLine.Should().BeNull();
        completedSession.AvailableMoves.Should().BeEmpty();
    }
}