using Microsoft.AspNetCore.Components;
using TicTacToe.Core.Models;
using TicTacToe.Core.Services;

namespace TicTacToe.Web.Pages;

/// <summary>
/// Hosts the single-screen tic-tac-toe experience.
/// </summary>
public partial class Home : ComponentBase
{
    [Inject]
    private IGameEngine GameEngine { get; set; } = default!;

    /// <summary>
    /// Gets the current session rendered by the page.
    /// </summary>
    protected GameSession Session { get; private set; } = GameSession.CreateNew();

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        Session = GameEngine.CreateNewSession();
    }

    /// <summary>
    /// Applies a player move to the current session.
    /// </summary>
    /// <param name="boardIndex">The zero-based board index selected by the user.</param>
    /// <returns>A completed task.</returns>
    protected Task HandleSpaceSelectionAsync(int boardIndex)
    {
        Session = GameEngine.PlayMove(Session, boardIndex);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Starts a fresh session.
    /// </summary>
    /// <returns>A completed task.</returns>
    protected Task RestartGameAsync()
    {
        Session = GameEngine.Restart(Session);
        return Task.CompletedTask;
    }
}