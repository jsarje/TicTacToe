namespace TicTacToe.Core.Tests;

public sealed class GameSessionStatusTests
{
    [Fact]
    public void StatusMessage_ShouldDescribeNewSession()
    {
        // Arrange
        var session = GameSession.CreateNew();

        // Act
        var message = session.StatusMessage;

        // Assert
        message.Should().Be("Player X to move.");
    }

    [Theory]
    [InlineData(GameResult.XWins, "Player X wins!")]
    [InlineData(GameResult.OWins, "Player O wins!")]
    [InlineData(GameResult.Draw, "It's a draw.")]
    public void StatusMessage_ShouldDescribeTerminalStates(GameResult result, string expectedMessage)
    {
        // Arrange
        var winningLine = result is GameResult.XWins or GameResult.OWins ? new[] { 0, 1, 2 } : null;
        var session = GameSessionTestData.CreateSession("XXXOO____", PlayerMark.X, result, winningLine);

        // Act
        var message = session.StatusMessage;

        // Assert
        message.Should().Be(expectedMessage);
    }
}