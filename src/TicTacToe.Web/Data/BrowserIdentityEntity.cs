namespace TicTacToe.Web.Data;

/// <summary>
/// Represents the persisted anonymous browser identity.
/// </summary>
public sealed class BrowserIdentityEntity
{
    public string BrowserId { get; set; } = string.Empty;

    public DateTimeOffset IssuedUtc { get; set; }

    public DateTimeOffset LastSeenUtc { get; set; }
}