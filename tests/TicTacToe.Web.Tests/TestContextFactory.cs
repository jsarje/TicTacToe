namespace TicTacToe.Web.Tests;

public static class TestContextFactory
{
    public static BunitContext Create()
    {
        var context = new BunitContext();
        context.Services.AddSingleton<IGameEngine, GameEngine>();
        return context;
    }
}