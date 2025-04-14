using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng Device
/// </summary>
public sealed class DeviceConfig : IEntityTypeConfiguration<Model.ReportSection.Device>
{
    public void Configure(EntityTypeBuilder<Model.ReportSection.Device> entity)
    {
        entity.ToTable("tbl_device");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);
    }
}