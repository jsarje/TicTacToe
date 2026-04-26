namespace TicTacToe.Web.Tests.Components;

public sealed class GameStatusTests
{
    [Fact]
    public void GameStatus_ShouldRenderLiveRegionForActiveState()
    {
        // Arrange
        using var context = TestContextFactory.Create();

        // Act
        var component = context.Render<GameStatus>(parameters => parameters.Add(parameter => parameter.Message, "Player X to move."));

        // Assert
        var status = component.Find("[role='status']");
        status.TextContent.Trim().Should().Be("Player X to move.");
        status.GetAttribute("aria-live").Should().Be("polite");
        status.GetAttribute("aria-atomic").Should().Be("true");
    }

    [Fact]
    public void GameStatus_ShouldRenderTerminalMessage()
    {
        // Arrange
        using var context = TestContextFactory.Create();

        // Act
        var component = context.Render<GameStatus>(parameters => parameters.Add(parameter => parameter.Message, "Player O wins!"));

        // Assert
        component.Markup.Should().Contain("Player O wins!");
    }
}