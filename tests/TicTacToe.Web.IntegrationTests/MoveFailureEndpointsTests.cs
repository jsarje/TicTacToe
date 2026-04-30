using Microsoft.AspNetCore.Mvc;

namespace TicTacToe.Web.IntegrationTests;

public sealed class MoveFailureEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public MoveFailureEndpointsTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task MoveEndpoint_ShouldReturnProblemDetailsWhenSaveFails()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var snapshot = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        factory.FaultInjectionState.TriggerSaveFailure();

        // Act
        var response = await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(0, PlayerMark.X, snapshot!.Revision));
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        problem!.Detail.Should().Contain("could not save the official match state");
    }

    [Fact]
    public async Task MoveEndpoint_ShouldPreserveLastConfirmedSnapshotWhenSaveFails()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var baseline = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        factory.FaultInjectionState.TriggerSaveFailure();

        // Act
        var failedResponse = await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(0, PlayerMark.X, baseline!.Revision));
        var recovered = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");

        // Assert
        failedResponse.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        recovered.Should().NotBeNull();
        recovered!.MatchId.Should().Be(baseline.MatchId);
        recovered.Revision.Should().Be(baseline.Revision);
        recovered.Board.Should().Equal(baseline.Board);
        recovered.CurrentPlayer.Should().Be(baseline.CurrentPlayer);
    }

    [Fact]
    public async Task MoveEndpoint_ShouldRejectStaleRevision()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var snapshot = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(0, PlayerMark.X, snapshot!.Revision));

        // Act
        var response = await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(1, PlayerMark.X, snapshot.Revision));
        var decision = await response.Content.ReadFromJsonAsync<MoveDecisionDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        decision!.Accepted.Should().BeFalse();
        decision.RejectionReason.Should().Be(MoveRejectionReason.StaleRevision);
    }

    [Fact]
    public async Task MoveEndpoint_ShouldRejectWrongTurn()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var snapshot = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");

        // Act
        var response = await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(0, PlayerMark.O, snapshot!.Revision));
        var decision = await response.Content.ReadFromJsonAsync<MoveDecisionDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        decision!.Accepted.Should().BeFalse();
        decision.RejectionReason.Should().Be(MoveRejectionReason.WrongTurn);
    }

    [Fact]
    public async Task MoveEndpoint_ShouldRejectCompletedMatch()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var snapshot = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(0, PlayerMark.X, snapshot!.Revision));
        var afterFirst = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(3, PlayerMark.O, afterFirst!.Revision));
        var afterSecond = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(1, PlayerMark.X, afterSecond!.Revision));
        var afterThird = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(4, PlayerMark.O, afterThird!.Revision));
        var afterFourth = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(2, PlayerMark.X, afterFourth!.Revision));
        var completed = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");

        // Act
        var response = await client.PostAsJsonAsync("/api/match/moves", new MoveRequestDto(8, PlayerMark.X, completed!.Revision));
        var decision = await response.Content.ReadFromJsonAsync<MoveDecisionDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        decision!.Accepted.Should().BeFalse();
        decision.RejectionReason.Should().Be(MoveRejectionReason.MatchComplete);
    }
}