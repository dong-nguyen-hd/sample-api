namespace AIRWAY.KIOSK.API.Domain.Context.Config;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Chức năng: cấu hình schema cho bảng RefreshToken
/// </summary>
public class RefreshTokenConfig : IEntityTypeConfiguration<Models.RefreshToken>
{
    public void Configure(EntityTypeBuilder<Models.RefreshToken> entity)
    {
        entity.ToTable("tbl_refresh_token");
        entity.Property(x => x.ExpiredUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);

        // Indexing
        entity.HasIndex(x => new { x.ExpiredUtc, x.IsUsed, x.Active });
    }
}
