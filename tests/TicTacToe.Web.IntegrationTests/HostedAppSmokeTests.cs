namespace TicTacToe.Web.IntegrationTests;

public sealed class HostedAppSmokeTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public HostedAppSmokeTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Root_ShouldServeHostedClientShell()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var html = await client.GetStringAsync("/");

        // Assert
        html.Should().Contain("<div id=\"app\">");
        html.Should().Contain("Tic-Tac-Toe");
    }
}