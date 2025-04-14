using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIRWAY.KIOSK.API.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng Passenger
/// </summary>
public sealed class PassengerConfig : IEntityTypeConfiguration<Model.Passenger>
{
    public void Configure(EntityTypeBuilder<Model.Passenger> entity)
    {
        entity.ToTable("tbl_passenger");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);
    }
}