using TicTacToe.Web.Data;

namespace TicTacToe.Web.Infrastructure;

/// <summary>
/// Resolves the browser identity from a persistent same-site cookie.
/// </summary>
public sealed class BrowserIdentityCookieService(IHttpContextAccessor httpContextAccessor, IMatchSessionRepository repository) : IBrowserIdentityService
{
    public const string CookieName = "ttt-browser";
    private static readonly TimeSpan CookieLifetime = TimeSpan.FromDays(30);

    /// <inheritdoc/>
    public async Task<BrowserIdentity> GetOrCreateAsync(CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("An active HTTP context is required.");
        var now = DateTimeOffset.UtcNow;
        var browserId = httpContext.Request.Cookies[CookieName];

        BrowserIdentityEntity entity;
        if (string.IsNullOrWhiteSpace(browserId))
        {
            entity = new BrowserIdentityEntity
            {
                BrowserId = Guid.NewGuid().ToString("N"),
                IssuedUtc = now,
                LastSeenUtc = now,
            };
        }
        else
        {
            entity = await repository.GetBrowserIdentityAsync(browserId, cancellationToken)
                ?? new BrowserIdentityEntity
                {
                    BrowserId = browserId,
                    IssuedUtc = now,
                    LastSeenUtc = now,
                };

            entity.LastSeenUtc = now;
        }

        await repository.UpsertBrowserIdentityAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        AppendCookie(httpContext, entity.BrowserId, now);

        return new BrowserIdentity(entity.BrowserId, entity.IssuedUtc, entity.LastSeenUtc);
    }

    private static void AppendCookie(HttpContext httpContext, string browserId, DateTimeOffset now)
    {
        httpContext.Response.Cookies.Append(CookieName, browserId, new CookieOptions
        {
            Expires = now.Add(CookieLifetime),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = httpContext.Request.IsHttps,
        });
    }
}