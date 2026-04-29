namespace TicTacToe.Web.Tests.Pages;

public sealed class HomeServerGameplayTests
{
    [Fact]
    public void Home_ShouldUpdateBoardFromServerConfirmedMove()
    {
        // Arrange
        var initial = TestContextFactory.CreateEmptyMatch();
        var updated = initial with
        {
            Revision = 1,
            Board = [PlayerMark.X, null, null, null, null, null, null, null, null],
            CurrentPlayer = PlayerMark.O,
            StatusMessage = "Player O to move.",
        };
        var client = new StubGameApiClient
        {
            LoadAsyncHandler = _ => Task.FromResult(initial),
            SubmitMoveAsyncHandler = (request, _) => Task.FromResult(new MoveDecisionDto(true, null, "Player O to move.", updated)),
            RestartAsyncHandler = _ => Task.FromResult(new RestartDecisionDto("Player X to move.", initial)),
        };
        using var context = TestContextFactory.Create(client);
        var component = context.Render<Home>();

        // Act
        component.FindAll(".game-board__space")[0].Click();

        // Assert
        component.FindAll(".game-board__space")[0].TextContent.Trim().Should().Be("X");
        component.Find("[role='status']").TextContent.Trim().Should().Be("Player O to move.");
    }

    [Fact]
    public void Home_ShouldRestartFromServerSnapshot()
    {
        // Arrange
        var completed = new MatchSnapshotDto(
            Guid.NewGuid(),
            5,
            [PlayerMark.X, PlayerMark.X, PlayerMark.X, PlayerMark.O, PlayerMark.O, null, null, null, null],
            PlayerMark.X,
            GameResult.XWins,
            [0, 1, 2],
            false,
            "Player X wins!",
            DateTimeOffset.UtcNow);
        var restarted = TestContextFactory.CreateEmptyMatch();
        var client = new StubGameApiClient
        {
            LoadAsyncHandler = _ => Task.FromResult(completed),
            SubmitMoveAsyncHandler = (_, _) => Task.FromResult(new MoveDecisionDto(false, MoveRejectionReason.MatchComplete, "This match is already complete. Restart to begin a new game.", completed)),
            RestartAsyncHandler = _ => Task.FromResult(new RestartDecisionDto("New game started. Player X to move.", restarted)),
        };
        using var context = TestContextFactory.Create(client);
        var component = context.Render<Home>();

        // Act
        component.Find(".restart-button").Click();

        // Assert
        component.Find("[role='status']").TextContent.Trim().Should().Be("New game started. Player X to move.");
        component.FindAll(".game-board__space").Should().OnlyContain(button => string.IsNullOrWhiteSpace(button.TextContent));
    }
}