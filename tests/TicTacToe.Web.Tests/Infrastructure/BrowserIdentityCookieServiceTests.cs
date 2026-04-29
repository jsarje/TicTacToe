using Microsoft.AspNetCore.Http;
using TicTacToe.Web.Data;
using TicTacToe.Web.Infrastructure;

namespace TicTacToe.Web.Tests.Infrastructure;

public sealed class BrowserIdentityCookieServiceTests
{
    [Fact]
    public async Task GetOrCreateAsync_ShouldIssueCookieAndPersistIdentity()
    {
        // Arrange
        var repository = new InMemoryMatchSessionRepository();
        var httpContext = new DefaultHttpContext();
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var service = new BrowserIdentityCookieService(accessor, repository);

        // Act
        var identity = await service.GetOrCreateAsync(CancellationToken.None);

        // Assert
        identity.BrowserId.Should().NotBeNullOrWhiteSpace();
        repository.Identities.Should().ContainKey(identity.BrowserId);
        httpContext.Response.Headers.SetCookie.ToString().Should().Contain(BrowserIdentityCookieService.CookieName);
    }

    private sealed class InMemoryMatchSessionRepository : IMatchSessionRepository
    {
        public Dictionary<string, BrowserIdentityEntity> Identities { get; } = [];

        public Task AddMatchAsync(TicTacToe.Web.Data.MatchSessionEntity match, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<BrowserIdentityEntity?> GetBrowserIdentityAsync(string browserId, CancellationToken cancellationToken)
        {
            Identities.TryGetValue(browserId, out var entity);
            return Task.FromResult(entity);
        }

        public Task<TicTacToe.Web.Data.MatchSessionEntity?> GetMatchAsync(string browserId, CancellationToken cancellationToken)
        {
            return Task.FromResult<TicTacToe.Web.Data.MatchSessionEntity?>(null);
        }

        public Task ReloadMatchAsync(TicTacToe.Web.Data.MatchSessionEntity match, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task UpsertBrowserIdentityAsync(BrowserIdentityEntity identity, CancellationToken cancellationToken)
        {
            Identities[identity.BrowserId] = identity;
            return Task.CompletedTask;
        }
    }
}