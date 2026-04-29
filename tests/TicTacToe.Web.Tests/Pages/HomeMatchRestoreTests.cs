namespace TicTacToe.Web.Tests.Pages;

public sealed class HomeMatchRestoreTests
{
    [Fact]
    public void Home_ShouldRenderPersistedBoardAfterInitialLoad()
    {
        // Arrange
        var persistedMatch = new MatchSnapshotDto(
            Guid.NewGuid(),
            3,
            [PlayerMark.X, PlayerMark.O, null, null, PlayerMark.X, null, null, null, null],
            PlayerMark.O,
            GameResult.InProgress,
            null,
            true,
            "Player O to move.",
            DateTimeOffset.UtcNow);
        using var context = TestContextFactory.Create(StubGameApiClient.WithMatch(persistedMatch));

        // Act
        var component = context.Render<Home>();

        // Assert
        component.Find("[role='status']").TextContent.Trim().Should().Be("Player O to move.");
        component.FindAll(".game-board__space")[0].TextContent.Trim().Should().Be("X");
        component.FindAll(".game-board__space")[1].TextContent.Trim().Should().Be("O");
        component.FindAll(".game-board__space")[4].TextContent.Trim().Should().Be("X");
    }

    [Fact]
    public void Home_ShouldRenderRetryStateWhenInitialLoadFails()
    {
        // Arrange
        var client = new StubGameApiClient
        {
            LoadAsyncHandler = _ => throw new GameApiException("The server could not load the official match state. Try again."),
        };
        using var context = TestContextFactory.Create(client);

        // Act
        var component = context.Render<Home>();

        // Assert
        component.Markup.Should().Contain("Board unavailable");
        component.Markup.Should().Contain("Retry");
    }
}