using TicTacToe.Core.Contracts;
using TicTacToe.Core.Models;
using TicTacToe.Web.Data;

namespace TicTacToe.Web.Infrastructure;

/// <summary>
/// Provides mapping between persisted match entities and shared contracts.
/// </summary>
public static class MatchMappingExtensions
{
    public static GameSession ToGameSession(this MatchSessionEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var board = entity.BoardState
            .Select((value, index) => new BoardSpace(index, value switch
            {
                'X' => PlayerMark.X,
                'O' => PlayerMark.O,
                _ => null,
            }))
            .ToArray();

        return new GameSession(board, entity.CurrentPlayer, entity.Result, ParseWinningLine(entity.WinningLineState));
    }

    public static MatchSnapshotDto ToSnapshotDto(this MatchSessionEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var session = entity.ToGameSession();

        return new MatchSnapshotDto(
            entity.MatchId,
            entity.Revision,
            session.Board.Select(space => space.Mark).ToArray(),
            session.CurrentPlayer,
            session.Result,
            session.WinningLine,
            session.IsBoardInteractive,
            session.StatusMessage,
            entity.LastUpdatedUtc);
    }

    public static MatchSessionEntity CreateNewMatch(string browserId, DateTimeOffset timestamp)
    {
        var session = GameSession.CreateNew();
        var entity = new MatchSessionEntity
        {
            BrowserId = browserId,
            MatchId = Guid.NewGuid(),
            CreatedUtc = timestamp,
        };

        entity.ApplySession(session, 0, timestamp, entity.MatchId, createdUtcOverride: timestamp);
        return entity;
    }

    public static void ApplySession(this MatchSessionEntity entity, GameSession session, int revision, DateTimeOffset updatedUtc, Guid? matchId = null, DateTimeOffset? createdUtcOverride = null)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(session);

        entity.MatchId = matchId ?? entity.MatchId;
        entity.BoardState = new string(session.Board.Select(space => space.Mark switch
        {
            PlayerMark.X => 'X',
            PlayerMark.O => 'O',
            _ => '_',
        }).ToArray());
        entity.CurrentPlayer = session.CurrentPlayer;
        entity.Result = session.Result;
        entity.WinningLineState = session.WinningLine is null ? null : string.Join(',', session.WinningLine);
        entity.Revision = revision;
        entity.LastUpdatedUtc = updatedUtc;
        entity.CreatedUtc = createdUtcOverride ?? entity.CreatedUtc;
    }

    private static IReadOnlyList<int>? ParseWinningLine(string? winningLineState)
    {
        if (string.IsNullOrWhiteSpace(winningLineState))
        {
            return null;
        }

        return winningLineState.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
    }
}