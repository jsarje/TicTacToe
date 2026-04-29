using Microsoft.AspNetCore.Components;
using TicTacToe.Core.Contracts;
using TicTacToe.Core.Models;
using TicTacToe.Web.Client.Services;

namespace TicTacToe.Web.Client.Pages;

/// <summary>
/// Hosts the single-screen authoritative tic-tac-toe experience.
/// </summary>
public partial class Home : ComponentBase
{
    [Inject]
    private IGameApiClient GameApiClient { get; set; } = default!;

    private MatchSnapshotDto? currentSnapshot;
    private MatchSnapshotDto? lastConfirmedSnapshot;
    private string statusMessage = "Loading official match...";
    private bool isLoading = true;
    private bool hasTransientFailure;

    protected MatchSnapshotDto? ActiveSnapshot => currentSnapshot ?? lastConfirmedSnapshot;

    protected IReadOnlyList<BoardSpace> Board => (ActiveSnapshot?.Board ?? Enumerable.Repeat<PlayerMark?>(null, 9))
        .Select((mark, index) => new BoardSpace(index, mark))
        .ToArray();

    protected bool IsBoardInteractive => ActiveSnapshot?.IsBoardInteractive is true && !isLoading && !hasTransientFailure;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await LoadMatchAsync();
    }

    protected async Task HandleSpaceSelectionAsync(int boardIndex)
    {
        if (ActiveSnapshot is null)
        {
            return;
        }

        isLoading = true;
        hasTransientFailure = false;
        statusMessage = "Confirming move with the server...";

        try
        {
            var decision = await GameApiClient.SubmitMoveAsync(new MoveRequestDto(boardIndex, ActiveSnapshot.CurrentPlayer, ActiveSnapshot.Revision));
            ApplySnapshot(decision.Match, decision.UserMessage);
        }
        catch (GameApiException exception)
        {
            HandleTransientFailure(exception.Message);
        }
        finally
        {
            isLoading = false;
        }
    }

    protected async Task RestartGameAsync()
    {
        isLoading = true;
        hasTransientFailure = false;
        statusMessage = "Starting a fresh official match...";

        try
        {
            var decision = await GameApiClient.RestartAsync();
            ApplySnapshot(decision.Match, decision.UserMessage);
        }
        catch (GameApiException exception)
        {
            HandleTransientFailure(exception.Message);
        }
        finally
        {
            isLoading = false;
        }
    }

    protected Task RetryLoadAsync()
    {
        return LoadMatchAsync();
    }

    private async Task LoadMatchAsync()
    {
        isLoading = true;
        hasTransientFailure = false;
        statusMessage = "Loading official match...";

        try
        {
            var snapshot = await GameApiClient.LoadMatchAsync();
            ApplySnapshot(snapshot, snapshot.StatusMessage);
        }
        catch (GameApiException exception)
        {
            HandleTransientFailure(exception.Message);
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ApplySnapshot(MatchSnapshotDto snapshot, string message)
    {
        currentSnapshot = snapshot;
        lastConfirmedSnapshot = snapshot;
        statusMessage = message;
        hasTransientFailure = false;
    }

    private void HandleTransientFailure(string message)
    {
        currentSnapshot = lastConfirmedSnapshot;
        hasTransientFailure = true;
        statusMessage = message;
    }
}