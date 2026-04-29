using System.Diagnostics;

namespace TicTacToe.Web.IntegrationTests;

public sealed class MoveEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public MoveEndpointsTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task MoveAndRestartEndpoints_ShouldReturnAuthoritativeSnapshots()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var snapshot = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");

        // Act
        var moveDecision = await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(0, PlayerMark.X, snapshot!.Revision));
        var movePayload = await moveDecision.Content.ReadFromJsonAsync<MoveDecisionDto>();
        var restartResponse = await client.PostAsync("/api/match/restart", null);
        var restartPayload = await restartResponse.Content.ReadFromJsonAsync<RestartDecisionDto>();

        // Assert
        movePayload!.Accepted.Should().BeTrue();
        movePayload.Match.Board[0].Should().Be(PlayerMark.X);
        movePayload.Match.CurrentPlayer.Should().Be(PlayerMark.O);
        restartPayload!.Match.Revision.Should().Be(0);
        restartPayload.Match.Board.Should().AllSatisfy(mark => mark.Should().BeNull());
    }

    [Fact]
    public async Task MoveEndpoint_ShouldStayWithinLatencyBudget()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var snapshot = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(0, PlayerMark.X, snapshot!.Revision));
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}