using TicTacToe.Core.Contracts;

namespace TicTacToe.Web.Tests;

public static class TestContextFactory
{
    public static BunitContext Create(StubGameApiClient? gameApiClient = null)
    {
        var context = new BunitContext();
        context.Services.AddSingleton<IGameApiClient>(gameApiClient ?? StubGameApiClient.WithMatch(CreateEmptyMatch()));
        return context;
    }

    public static MatchSnapshotDto CreateEmptyMatch(int revision = 0)
    {
        return new MatchSnapshotDto(
            Guid.NewGuid(),
            revision,
            Enumerable.Repeat<PlayerMark?>(null, 9).ToArray(),
            PlayerMark.X,
            GameResult.InProgress,
            null,
            true,
            "Player X to move.",
            DateTimeOffset.UtcNow);
    }
}