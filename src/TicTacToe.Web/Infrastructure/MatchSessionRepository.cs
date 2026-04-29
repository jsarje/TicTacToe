using Microsoft.EntityFrameworkCore;
using TicTacToe.Web.Data;

namespace TicTacToe.Web.Infrastructure;

/// <summary>
/// Implements persisted match and browser identity data access.
/// </summary>
public sealed class MatchSessionRepository(TicTacToeDbContext dbContext, RepositoryFaultInjectionState faultInjectionState) : IMatchSessionRepository
{
    /// <inheritdoc/>
    public async Task<MatchSessionEntity?> GetMatchAsync(string browserId, CancellationToken cancellationToken)
    {
        ThrowIfLoadFaultRequested();

        try
        {
            return await dbContext.MatchSessions.SingleOrDefaultAsync(entity => entity.BrowserId == browserId, cancellationToken);
        }
        catch (Exception exception) when (exception is not MatchPersistenceException)
        {
            throw new MatchPersistenceException("The server could not load the official match state. Try again.", exception);
        }
    }

    /// <inheritdoc/>
    public async Task<BrowserIdentityEntity?> GetBrowserIdentityAsync(string browserId, CancellationToken cancellationToken)
    {
        ThrowIfLoadFaultRequested();

        try
        {
            return await dbContext.BrowserIdentities.SingleOrDefaultAsync(entity => entity.BrowserId == browserId, cancellationToken);
        }
        catch (Exception exception) when (exception is not MatchPersistenceException)
        {
            throw new MatchPersistenceException("The server could not load the browser identity. Try again.", exception);
        }
    }

    /// <inheritdoc/>
    public Task AddMatchAsync(MatchSessionEntity match, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(match);
        return dbContext.MatchSessions.AddAsync(match, cancellationToken).AsTask();
    }

    /// <inheritdoc/>
    public Task UpsertBrowserIdentityAsync(BrowserIdentityEntity identity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(identity);

        var tracked = dbContext.BrowserIdentities.Local.SingleOrDefault(entity => entity.BrowserId == identity.BrowserId);
        if (tracked is not null)
        {
            tracked.IssuedUtc = identity.IssuedUtc;
            tracked.LastSeenUtc = identity.LastSeenUtc;
            return Task.CompletedTask;
        }

        var existing = dbContext.BrowserIdentities.SingleOrDefault(entity => entity.BrowserId == identity.BrowserId);
        if (existing is not null)
        {
            existing.IssuedUtc = identity.IssuedUtc;
            existing.LastSeenUtc = identity.LastSeenUtc;
            return Task.CompletedTask;
        }

        dbContext.BrowserIdentities.Add(identity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        if (faultInjectionState.ConsumeSaveFailure())
        {
            throw new MatchPersistenceException("The server could not save the official match state. Try again.");
        }

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
        catch (Exception exception) when (exception is not MatchPersistenceException)
        {
            throw new MatchPersistenceException("The server could not save the official match state. Try again.", exception);
        }
    }

    /// <inheritdoc/>
    public Task ReloadMatchAsync(MatchSessionEntity match, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(match);
        return dbContext.Entry(match).ReloadAsync(cancellationToken);
    }

    private void ThrowIfLoadFaultRequested()
    {
        if (faultInjectionState.ConsumeLoadFailure())
        {
            throw new MatchPersistenceException("The server could not load the official match state. Try again.");
        }
    }
}