using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TicTacToe.Web.Data.Configurations;

/// <summary>
/// Configures persistence for the authoritative match entity.
/// </summary>
public sealed class MatchSessionEntityConfiguration : IEntityTypeConfiguration<MatchSessionEntity>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<MatchSessionEntity> builder)
    {
        builder.HasKey(entity => entity.BrowserId);
        builder.Property(entity => entity.BrowserId).HasMaxLength(128);
        builder.Property(entity => entity.BoardState).HasMaxLength(9).IsRequired();
        builder.Property(entity => entity.WinningLineState).HasMaxLength(32);
        builder.Property(entity => entity.Revision).IsConcurrencyToken();
    }
}