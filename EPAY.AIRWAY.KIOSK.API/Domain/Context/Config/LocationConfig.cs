using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng Location
/// </summary>
public sealed class LocationConfig : IEntityTypeConfiguration<Model.ReportSection.Location>
{
    public void Configure(EntityTypeBuilder<Model.ReportSection.Location> entity)
    {
        entity.ToTable("tbl_location");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);
    }
}