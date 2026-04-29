namespace TicTacToe.Web.Infrastructure;

/// <summary>
/// Resolves or creates the current browser identity from the request cookie boundary.
/// </summary>
public interface IBrowserIdentityService
{
    /// <summary>
    /// Gets or creates the current browser identity and renews its cookie lifetime.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The active browser identity.</returns>
    Task<BrowserIdentity> GetOrCreateAsync(CancellationToken cancellationToken);
}