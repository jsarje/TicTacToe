using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TicTacToe.Web.IntegrationTests;

public sealed class MatchLoadEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public MatchLoadEndpointsTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetMatch_ShouldCreateAndReloadSameBrowserMatch()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });

        // Act
        var firstResponse = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        var secondResponse = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");

        // Assert
        firstResponse.Should().NotBeNull();
        secondResponse.Should().NotBeNull();
        secondResponse!.MatchId.Should().Be(firstResponse!.MatchId);
        secondResponse.Revision.Should().Be(0);
    }

    [Fact]
    public async Task GetMatch_ShouldStayWithinLoadBudget()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await client.GetAsync("/api/match");
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetMatch_ShouldReturnSamePersistedSnapshotAfterTransientLoadFailure()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var baseline = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        factory.FaultInjectionState.TriggerLoadFailure();

        // Act
        var failedResponse = await client.GetAsync("/api/match");
        var problem = await failedResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        var recovered = await client.GetFromJsonAsync<MatchSnapshotDto>("/api/match");

        // Assert
        failedResponse.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        problem!.Detail.Should().Contain("could not load the official match state");
        recovered.Should().NotBeNull();
        recovered!.MatchId.Should().Be(baseline!.MatchId);
        recovered.Revision.Should().Be(baseline.Revision);
        recovered.Board.Should().Equal(baseline.Board);
    }
}