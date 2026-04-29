namespace TicTacToe.Web.Infrastructure;

/// <summary>
/// Represents the active browser identity used to scope match state.
/// </summary>
/// <param name="BrowserId">The opaque browser identifier.</param>
/// <param name="IssuedUtc">The identity issue time.</param>
/// <param name="LastSeenUtc">The most recent successful request time.</param>
public sealed record BrowserIdentity(string BrowserId, DateTimeOffset IssuedUtc, DateTimeOffset LastSeenUtc);