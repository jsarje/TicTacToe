using Microsoft.EntityFrameworkCore;
using TicTacToe.Core.Contracts;
using TicTacToe.Core.Models;
using TicTacToe.Core.Services;
using TicTacToe.Web.Data;

namespace TicTacToe.Web.Infrastructure;

/// <summary>
/// Implements the authoritative match load, move, and restart workflow.
/// </summary>
public sealed class MatchRefereeService(
    IGameEngine gameEngine,
    IBrowserIdentityService browserIdentityService,
    IMatchSessionRepository repository) : IMatchRefereeService
{
    /// <inheritdoc/>
    public async Task<MatchSnapshotDto> LoadMatchAsync(CancellationToken cancellationToken)
    {
        var browserIdentity = await browserIdentityService.GetOrCreateAsync(cancellationToken);
        var match = await GetOrCreateMatchAsync(browserIdentity.BrowserId, cancellationToken);
        return match.ToSnapshotDto();
    }

    /// <inheritdoc/>
    public async Task<MoveDecisionDto> ApplyMoveAsync(MoveRequestDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var browserIdentity = await browserIdentityService.GetOrCreateAsync(cancellationToken);
        var match = await GetOrCreateMatchAsync(browserIdentity.BrowserId, cancellationToken);

        if (request.ExpectedRevision != match.Revision)
        {
            return new MoveDecisionDto(false, MoveRejectionReason.StaleRevision, MoveDecisionMessageFactory.ForRejection(MoveRejectionReason.StaleRevision), match.ToSnapshotDto());
        }

        var result = gameEngine.EvaluateMove(match.ToGameSession(), request.SpaceIndex, request.ActingPlayer);
        if (!result.Accepted)
        {
            return new MoveDecisionDto(false, result.RejectionReason, MoveDecisionMessageFactory.ForRejection(result.RejectionReason!.Value, request.SpaceIndex), match.ToSnapshotDto());
        }

        var now = DateTimeOffset.UtcNow;
        match.ApplySession(result.Session, match.Revision + 1, now);

        try
        {
            await repository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await repository.ReloadMatchAsync(match, cancellationToken);
            return new MoveDecisionDto(false, MoveRejectionReason.StaleRevision, MoveDecisionMessageFactory.ForRejection(MoveRejectionReason.StaleRevision), match.ToSnapshotDto());
        }

        return new MoveDecisionDto(true, null, MoveDecisionMessageFactory.ForAcceptedMove(result.Session), match.ToSnapshotDto());
    }

    /// <inheritdoc/>
    public async Task<RestartDecisionDto> RestartAsync(CancellationToken cancellationToken)
    {
        var browserIdentity = await browserIdentityService.GetOrCreateAsync(cancellationToken);
        var match = await GetOrCreateMatchAsync(browserIdentity.BrowserId, cancellationToken);
        var now = DateTimeOffset.UtcNow;

        match.ApplySession(gameEngine.CreateNewSession(), 0, now, Guid.NewGuid(), createdUtcOverride: now);
        await repository.SaveChangesAsync(cancellationToken);

        return new RestartDecisionDto(MoveDecisionMessageFactory.ForRestart(), match.ToSnapshotDto());
    }

    private async Task<MatchSessionEntity> GetOrCreateMatchAsync(string browserId, CancellationToken cancellationToken)
    {
        var match = await repository.GetMatchAsync(browserId, cancellationToken);
        if (match is not null)
        {
            return match;
        }

        var createdMatch = MatchMappingExtensions.CreateNewMatch(browserId, DateTimeOffset.UtcNow);
        await repository.AddMatchAsync(createdMatch, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return createdMatch;
    }
}