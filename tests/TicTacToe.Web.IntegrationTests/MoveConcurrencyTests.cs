namespace TicTacToe.Web.IntegrationTests;

public sealed class MoveConcurrencyTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public MoveConcurrencyTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task MoveEndpoint_ShouldRejectConflictingSecondMoveForStaleRevision()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var snapshot = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");

        // Act
        var firstDecision = await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(0, PlayerMark.X, snapshot!.Revision));
        var secondDecision = await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(1, PlayerMark.X, snapshot.Revision));
        var secondPayload = await secondDecision.Content.ReadFromJsonAsync<MoveDecisionDto>();

        // Assert
        firstDecision.StatusCode.Should().Be(HttpStatusCode.OK);
        secondPayload!.Accepted.Should().BeFalse();
        secondPayload.RejectionReason.Should().Be(MoveRejectionReason.StaleRevision);
    }
}