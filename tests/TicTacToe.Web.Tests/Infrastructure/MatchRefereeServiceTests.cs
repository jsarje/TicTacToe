using Microsoft.EntityFrameworkCore;
using TicTacToe.Core.Contracts;
using TicTacToe.Core.Services;
using TicTacToe.Web.Data;
using TicTacToe.Web.Infrastructure;

namespace TicTacToe.Web.Tests.Infrastructure;

public sealed class MatchRefereeServiceTests
{
    [Fact]
    public async Task ApplyMoveAsync_ShouldRejectStaleRevision()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new MatchSessionRepository(dbContext, new RepositoryFaultInjectionState());
        var browserIdentityService = new StubBrowserIdentityService("browser-1");
        var service = new MatchRefereeService(new GameEngine(), browserIdentityService, repository);
        dbContext.MatchSessions.Add(MatchMappingExtensions.CreateNewMatch("browser-1", DateTimeOffset.UtcNow));
        await dbContext.SaveChangesAsync();

        // Act
        var decision = await service.ApplyMoveAsync(new MoveRequestDto(0, PlayerMark.X, 99), CancellationToken.None);

        // Assert
        decision.Accepted.Should().BeFalse();
        decision.RejectionReason.Should().Be(MoveRejectionReason.StaleRevision);
    }

    private static TicTacToeDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TicTacToeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new TicTacToeDbContext(options);
    }

    private sealed class StubBrowserIdentityService(string browserId) : IBrowserIdentityService
    {
        public Task<BrowserIdentity> GetOrCreateAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new BrowserIdentity(browserId, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
        }
    }
}