using TicTacToe.Core.Contracts;
using TicTacToe.Web.Client.Services;

namespace TicTacToe.Web.Tests;

public sealed class StubGameApiClient : IGameApiClient
{
    public Func<CancellationToken, Task<MatchSnapshotDto>> LoadAsyncHandler { get; set; } = _ => throw new NotImplementedException();

    public Func<MoveRequestDto, CancellationToken, Task<MoveDecisionDto>> SubmitMoveAsyncHandler { get; set; } = (_, _) => throw new NotImplementedException();

    public Func<CancellationToken, Task<RestartDecisionDto>> RestartAsyncHandler { get; set; } = _ => throw new NotImplementedException();

    public Task<MatchSnapshotDto> LoadMatchAsync(CancellationToken cancellationToken = default)
    {
        return LoadAsyncHandler(cancellationToken);
    }

    public Task<MoveDecisionDto> SubmitMoveAsync(MoveRequestDto request, CancellationToken cancellationToken = default)
    {
        return SubmitMoveAsyncHandler(request, cancellationToken);
    }

    public Task<RestartDecisionDto> RestartAsync(CancellationToken cancellationToken = default)
    {
        return RestartAsyncHandler(cancellationToken);
    }

    public static StubGameApiClient WithMatch(MatchSnapshotDto snapshot)
    {
        return new StubGameApiClient
        {
            LoadAsyncHandler = _ => Task.FromResult(snapshot),
            SubmitMoveAsyncHandler = (_, _) => Task.FromResult(new MoveDecisionDto(true, null, snapshot.StatusMessage, snapshot)),
            RestartAsyncHandler = _ => Task.FromResult(new RestartDecisionDto(snapshot.StatusMessage, snapshot)),
        };
    }
}