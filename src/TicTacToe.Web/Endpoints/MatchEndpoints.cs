using TicTacToe.Core.Contracts;
using TicTacToe.Web.Infrastructure;

namespace TicTacToe.Web.Endpoints;

/// <summary>
/// Maps the authoritative match endpoints.
/// </summary>
public static class MatchEndpoints
{
    public static IEndpointRouteBuilder MapMatchEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/match");

        group.MapGet(string.Empty, async (IMatchRefereeService service, CancellationToken cancellationToken) =>
        {
            return Results.Ok(await service.LoadMatchAsync(cancellationToken));
        });

        group.MapPost("/moves", async (MoveRequestDto request, IMatchRefereeService service, CancellationToken cancellationToken) =>
        {
            return Results.Ok(await service.ApplyMoveAsync(request, cancellationToken));
        });

        group.MapPost("/restart", async (IMatchRefereeService service, CancellationToken cancellationToken) =>
        {
            return Results.Ok(await service.RestartAsync(cancellationToken));
        });

        return endpoints;
    }
}