namespace TicTacToe.Web.Tests.Pages;

public sealed class HomeGameplayTests
{
    [Fact]
    public void Home_ShouldPlayAFullWinningMatch()
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

        // Assert
        component.Find("[role='status']").TextContent.Trim().Should().Be("Player X wins!");
        component.FindAll(".game-board__space").Should().OnlyContain(button => button.HasAttribute("disabled"));
        component.FindAll(".game-board__space.game-board__space--winning").Select(button => button.TextContent.Trim()).Should().OnlyContain(value => value == "X");
    }

    [Fact]
    public void Home_ShouldIgnoreRepeatSelectionsAndBlockPostGameMoves()
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

        // Assert
        component.FindAll(".game-board__space")[0].TextContent.Trim().Should().Be("X");
        component.FindAll(".game-board__space").Count(button => button.TextContent.Trim() == "O").Should().Be(2);
        component.FindAll(".game-board__space")[8].TextContent.Trim().Should().BeEmpty();
        component.FindAll(".game-board__space")[0].HasAttribute("disabled").Should().BeTrue();
        component.FindAll(".game-board__space")[8].HasAttribute("disabled").Should().BeTrue();
        component.Find("[role='status']").TextContent.Trim().Should().Be("Player X wins!");
    }
}