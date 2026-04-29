namespace TicTacToe.Web.Tests.Pages;

public sealed class HomeFailureStateTests
{
    [Fact]
    public void Home_ShouldKeepLastConfirmedBoardWhenMoveSubmissionFails()
    {
        // Arrange
        var confirmed = new MatchSnapshotDto(
            Guid.NewGuid(),
            2,
            [PlayerMark.X, PlayerMark.O, null, null, null, null, null, null, null],
            PlayerMark.X,
            GameResult.InProgress,
            null,
            true,
            "Player X to move.",
            DateTimeOffset.UtcNow);
        var client = new StubGameApiClient
        {
            LoadAsyncHandler = _ => Task.FromResult(confirmed),
            SubmitMoveAsyncHandler = (_, _) => throw new GameApiException("The server could not save the official match state. Try again."),
            RestartAsyncHandler = _ => Task.FromResult(new RestartDecisionDto("Player X to move.", confirmed)),
        };
        using var context = TestContextFactory.Create(client);
        var component = context.Render<Home>();

        // Act
        component.FindAll(".game-board__space")[2].Click();

        // Assert
        component.FindAll(".game-board__space")[0].TextContent.Trim().Should().Be("X");
        component.FindAll(".game-board__space")[1].TextContent.Trim().Should().Be("O");
        component.Markup.Should().Contain("Retry sync");
        component.Markup.Should().Contain("The server could not save the official match state. Try again.");
    }

    [Fact]
    public void Home_ShouldRenderRejectionMessageFromServer()
    {
        // Arrange
        var stale = new MatchSnapshotDto(
            Guid.NewGuid(),
            4,
            [PlayerMark.X, PlayerMark.O, null, null, PlayerMark.X, null, null, null, null],
            PlayerMark.O,
            GameResult.InProgress,
            null,
            true,
            "Player O to move.",
            DateTimeOffset.UtcNow);
        var client = new StubGameApiClient
        {
            LoadAsyncHandler = _ => Task.FromResult(stale),
            SubmitMoveAsyncHandler = (_, _) => Task.FromResult(new MoveDecisionDto(false, MoveRejectionReason.StaleRevision, "Your board was out of date. The latest official state has been reloaded.", stale)),
            RestartAsyncHandler = _ => Task.FromResult(new RestartDecisionDto("Player X to move.", stale)),
        };
        using var context = TestContextFactory.Create(client);
        var component = context.Render<Home>();

        // Act
        component.FindAll(".game-board__space")[2].Click();

        // Assert
        component.Markup.Should().Contain("Your board was out of date. The latest official state has been reloaded.");
        component.FindAll(".game-board__space")[4].TextContent.Trim().Should().Be("X");
    }

    [Fact]
    public void Home_ShouldKeepStatusLiveRegionForAssistiveAnnouncements()
    {
        // Arrange
        var snapshot = TestContextFactory.CreateEmptyMatch();
        using var context = TestContextFactory.Create(StubGameApiClient.WithMatch(snapshot));

        // Act
        var component = context.Render<Home>();

        // Assert
        var status = component.Find("[role='status']");
        status.GetAttribute("aria-live").Should().Be("polite");
        status.GetAttribute("aria-atomic").Should().Be("true");
    }
}