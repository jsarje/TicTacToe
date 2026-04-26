namespace TicTacToe.Core.Tests;

public sealed class GameEngineMoveTests
{
    private readonly GameEngine gameEngine = new();

    [Fact]
    public void PlayMove_ShouldRecordMoveAndAlternateTurn()
    {
        // Arrange
        var session = gameEngine.CreateNewSession();

        // Act
        var updatedSession = gameEngine.PlayMove(session, 0);

        // Assert
        updatedSession.Board[0].Mark.Should().Be(PlayerMark.X);
        updatedSession.CurrentPlayer.Should().Be(PlayerMark.O);
        updatedSession.Result.Should().Be(GameResult.InProgress);
    }

    [Fact]
    public void PlayMove_ShouldIgnoreOccupiedSpace()
    {
        // Arrange
        var session = gameEngine.PlayMove(gameEngine.CreateNewSession(), 0);

        // Act
        var unchangedSession = gameEngine.PlayMove(session, 0);

        // Assert
        unchangedSession.Should().BeSameAs(session);
        unchangedSession.CurrentPlayer.Should().Be(PlayerMark.O);
        unchangedSession.Board.Count(space => space.Mark is not null).Should().Be(1);
    }
}