namespace TicTacToe.Web.Tests.Pages;

public sealed class HomeRestartTests
{
    [Fact]
    public void Home_ShouldRestartFromAnActiveMatch()
    {
        // Arrange
        using var context = TestContextFactory.Create();
        var component = context.Render<Home>();

        void ClickSpace(int index)
        {
            component.FindAll(".game-board__space")[index].Click();
        }

        // Act
        ClickSpace(0);
        component.Find(".restart-button").Click();

        // Assert
        component.Find("[role='status']").TextContent.Trim().Should().Be("Player X to move.");
        component.FindAll(".game-board__space").Should().OnlyContain(button => string.IsNullOrWhiteSpace(button.TextContent));
        component.FindAll(".game-board__space").Should().NotContain(button => button.HasAttribute("disabled"));
    }

    [Fact]
    public void Home_ShouldRestartFromACompletedMatch()
    {
        // Arrange
        using var context = TestContextFactory.Create();
        var component = context.Render<Home>();

        void ClickSpace(int index)
        {
            component.FindAll(".game-board__space")[index].Click();
        }

        // Act
        ClickSpace(0);
        ClickSpace(3);
        ClickSpace(1);
        ClickSpace(4);
        ClickSpace(2);
        component.Find(".restart-button").Click();

        // Assert
        component.Find("[role='status']").TextContent.Trim().Should().Be("Player X to move.");
        component.FindAll(".game-board__space").Should().OnlyContain(button => string.IsNullOrWhiteSpace(button.TextContent));
        component.FindAll(".game-board__space").Should().NotContain(button => button.HasAttribute("disabled"));
    }
}