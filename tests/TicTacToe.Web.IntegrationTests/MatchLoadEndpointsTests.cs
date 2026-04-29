using System.Diagnostics;

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
}