using TicTacToe.Web.Infrastructure;

namespace TicTacToe.Web.Tests.Infrastructure;

public sealed class MatchMappingExtensionsTests
{
    [Fact]
    public void ToSnapshotDto_ShouldMapPersistedEntity()
    {
        // Arrange
        var entity = new TicTacToe.Web.Data.MatchSessionEntity
        {
            MatchId = Guid.NewGuid(),
            BrowserId = "browser-1",
            BoardState = "XO__X____",
            CurrentPlayer = PlayerMark.O,
            Result = GameResult.InProgress,
            Revision = 3,
            CreatedUtc = DateTimeOffset.UtcNow,
            LastUpdatedUtc = DateTimeOffset.UtcNow,
        };

        // Act
        var snapshot = entity.ToSnapshotDto();

        // Assert
        snapshot.Board[0].Should().Be(PlayerMark.X);
        snapshot.Board[1].Should().Be(PlayerMark.O);
        snapshot.Board[4].Should().Be(PlayerMark.X);
        snapshot.Revision.Should().Be(3);
    }
}