namespace TicTacToe.Core.Tests;

public sealed class GameEngineRestartTests
{
    private readonly GameEngine gameEngine = new();

    [Fact]
    public void Restart_ShouldResetAnActiveSession()
    {
        // Arrange
        var session = gameEngine.PlayMove(gameEngine.CreateNewSession(), 4);

        // Act
        var restartedSession = gameEngine.Restart(session);

        // Assert
        restartedSession.Result.Should().Be(GameResult.InProgress);
        restartedSession.CurrentPlayer.Should().Be(PlayerMark.X);
        restartedSession.Board.Should().AllSatisfy(space => space.Mark.Should().BeNull());
        restartedSession.WinningLine.Should().BeNull();
    }

    [Fact]
    public void Restart_ShouldResetACompletedSession()
    {
        // Arrange
        var completedSession = GameSessionTestData.CreateSession("XXXOO____", PlayerMark.X, GameResult.XWins, new[] { 0, 1, 2 });

        // Act
        var restartedSession = gameEngine.Restart(completedSession);

        // Assert
        restartedSession.Result.Should().Be(GameResult.InProgress);
        restartedSession.CurrentPlayer.Should().Be(PlayerMark.X);
        restartedSession.Board.Should().AllSatisfy(space => space.Mark.Should().BeNull());
        restartedSession.WinningLine.Should().BeNull();
    }
}