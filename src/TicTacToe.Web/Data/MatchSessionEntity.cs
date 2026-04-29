using TicTacToe.Core.Models;

namespace TicTacToe.Web.Data;

/// <summary>
/// Represents the persisted authoritative match for one browser.
/// </summary>
public sealed class MatchSessionEntity
{
    public Guid MatchId { get; set; }

    public string BrowserId { get; set; } = string.Empty;

    public string BoardState { get; set; } = "_________";

    public PlayerMark CurrentPlayer { get; set; } = PlayerMark.X;

    public GameResult Result { get; set; } = GameResult.InProgress;

    public string? WinningLineState { get; set; }

    public int Revision { get; set; }

    public DateTimeOffset CreatedUtc { get; set; }

    public DateTimeOffset LastUpdatedUtc { get; set; }
}