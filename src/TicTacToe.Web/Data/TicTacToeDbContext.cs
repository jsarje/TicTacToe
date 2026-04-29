using Microsoft.EntityFrameworkCore;
using TicTacToe.Web.Data.Configurations;

namespace TicTacToe.Web.Data;

/// <summary>
/// Provides database access for persisted match and browser identity state.
/// </summary>
public sealed class TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options) : DbContext(options)
{
    public DbSet<MatchSessionEntity> MatchSessions => Set<MatchSessionEntity>();

    public DbSet<BrowserIdentityEntity> BrowserIdentities => Set<BrowserIdentityEntity>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MatchSessionEntityConfiguration());

        modelBuilder.Entity<BrowserIdentityEntity>(builder =>
        {
            builder.HasKey(entity => entity.BrowserId);
            builder.Property(entity => entity.BrowserId).HasMaxLength(128);
        });
    }
}