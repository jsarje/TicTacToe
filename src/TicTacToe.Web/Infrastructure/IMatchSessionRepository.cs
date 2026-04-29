using TicTacToe.Web.Data;

namespace TicTacToe.Web.Infrastructure;

/// <summary>
/// Provides database operations for match and browser identity state.
/// </summary>
public interface IMatchSessionRepository
{
    /// <summary>
    /// Loads the persisted match for a browser.
    /// </summary>
    Task<MatchSessionEntity?> GetMatchAsync(string browserId, CancellationToken cancellationToken);

    /// <summary>
    /// Loads the persisted browser identity record.
    /// </summary>
    Task<BrowserIdentityEntity?> GetBrowserIdentityAsync(string browserId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new match entity to the current unit of work.
    /// </summary>
    Task AddMatchAsync(MatchSessionEntity match, CancellationToken cancellationToken);

    /// <summary>
    /// Adds or updates the browser identity in the current unit of work.
    /// </summary>
    Task UpsertBrowserIdentityAsync(BrowserIdentityEntity identity, CancellationToken cancellationToken);

    /// <summary>
    /// Persists the current unit of work.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Reloads the tracked match entity from the database after a concurrency conflict.
    /// </summary>
    Task ReloadMatchAsync(MatchSessionEntity match, CancellationToken cancellationToken);
}