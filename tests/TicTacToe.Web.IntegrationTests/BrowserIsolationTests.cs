namespace TicTacToe.Web.IntegrationTests;

public sealed class BrowserIsolationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public BrowserIsolationTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetMatch_ShouldIsolateBrowsersByCookieContainer()
    {
        // Arrange
        using var firstClient = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        using var secondClient = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });

        // Act
        var firstMatch = await firstClient.GetFromJsonAsync<MatchSnapshotDto>("/api/match");
        var secondMatch = await secondClient.GetFromJsonAsync<MatchSnapshotDto>("/api/match");

        // Assert
        firstMatch.Should().NotBeNull();
        secondMatch.Should().NotBeNull();
        secondMatch!.MatchId.Should().NotBe(firstMatch!.MatchId);
    }
}